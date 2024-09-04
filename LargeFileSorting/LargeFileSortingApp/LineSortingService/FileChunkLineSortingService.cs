using System.Collections.Concurrent;
using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.Utils;

namespace LargeFileSortingApp.LineSortingService;

public class LineFromDump
{
    public string File { get; set; }
    public LineItem LineItem { get; set; }
    
    public LineFromDump(string file, LineItem lineItem)
    {
        File = file;
        LineItem = lineItem;
    }
}

public class FileChunkLineSortingService : ILineSortingService, IDisposable
{
    private const string TempFolderName = "tmp_dump"; // TODO: add guid to suffix

    private readonly IFileChunkLineReader _chunkLineReader;

    private readonly int _bufferSizeB;

    public FileChunkLineSortingService(IFileChunkLineReader chunkLineReader, int fileOpBufferSizeB)
    {
        _chunkLineReader = chunkLineReader;
        _bufferSizeB = fileOpBufferSizeB;
    }

    public IEnumerable<LineItem> GetSortedLines()
    {
        var chunks = _chunkLineReader.ReadChunks();
        
        var chunkFiles = SortAndDumpToFiles(chunks);
        
        foreach (var item in MergeFiles(chunkFiles))
        {
            yield return item;
        }
        
        foreach (var file in chunkFiles)
        {
            File.Delete(file);
        }

        //TODO: race condition on double run at the same time
        if (!Directory.EnumerateFileSystemEntries(TempFolderName).Any())
        {
            Directory.Delete(TempFolderName);
        }
    }

    private string[] SortAndDumpToFiles(IEnumerable<LineItem[]> chunks)
    {
        var producerCount = 6;
        var sortedChunkBlockedQueue = new BlockingCollection<LineItem[]>(producerCount);
        var partsToSort = chunks.Chunk(producerCount);
        
        var sortWorkerTasks = new List<Task>();
        foreach (var part in partsToSort)
        {
            var sortWorkerTask = Task.Factory.StartNew(() =>
            {
                foreach (var chunk in part)
                {
                    var sortedChunk = chunk.SortInMemory();
                    sortedChunkBlockedQueue.Add(sortedChunk);
                }
            });
            sortWorkerTasks.Add(sortWorkerTask);
        }

        var dumpToFileWorkerTask = Task.Factory.StartNew<string[]>(() =>
        {
            var files = new List<string>();
            
            if (!Directory.Exists(TempFolderName))
                Directory.CreateDirectory(TempFolderName);
            
            var dumpWriter = new FileLineWriter();
            do
            {
                var sortedChunk = sortedChunkBlockedQueue.Take();
                var uniqueName = Guid.NewGuid().ToString(); // TODO: handle collision
                var dumpFile = Path.Combine(TempFolderName, uniqueName);
                dumpWriter.WriteLines(dumpFile, sortedChunk);
                files.Add(dumpFile);
            } while (!sortedChunkBlockedQueue.IsAddingCompleted || sortedChunkBlockedQueue.Count > 0);

            return files.ToArray();
        });
        
        Task.WaitAll(sortWorkerTasks.ToArray());
        sortedChunkBlockedQueue.CompleteAdding();

        return dumpToFileWorkerTask.Result;
    }
    
    private IEnumerable<LineItem> MergeFiles(string[] files)
    {
        var fileToIterators = new Dictionary<string, IEnumerator<LineItem>>();
        try
        {
            var heap = new PriorityQueue<LineFromDump, LineItem>();
            foreach (var file in files)
            {
                string.Intern(file);
                var reader = new FileLineReader();
                var fileEnumerator = reader.ReadLines(file).GetEnumerator();
                if (fileEnumerator.MoveNext())
                {   
                    var current = fileEnumerator.Current;
                    var item = new LineFromDump(file, current);
                    heap.Enqueue(item, current);
                    fileToIterators[file] = fileEnumerator;
                }
            }

            while (heap.TryDequeue(out var item, out var pair))
            {
                yield return pair;
                var iterator = fileToIterators[item.File];
                if (iterator.MoveNext())
                {
                    var nextCurrent = iterator.Current;
                    var nextItem = new LineFromDump(item.File, nextCurrent);
                    heap.Enqueue(nextItem, nextCurrent);
                }
            }
        }
        finally
        {
            foreach (var fileIterators in fileToIterators.Values)
            {
                fileIterators.Dispose();
            }
        }

    }

    public void Dispose()
    {
        _chunkLineReader.Dispose();
    }
}