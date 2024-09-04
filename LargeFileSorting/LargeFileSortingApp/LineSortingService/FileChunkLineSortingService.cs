using System.Collections.Concurrent;
using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.Utils;

namespace LargeFileSortingApp.LineSortingService;

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
    
    private IEnumerable<LineItem> MergeFiles(string[] filenames)
    {
        var fileToIterators = new Dictionary<string, IEnumerator<LineItem>>();
        try
        {
            var heap = new PriorityQueue<string, LineItem>();
            foreach (var filename in filenames)
            {
                string.Intern(filename);
                var reader = new FileLineReader();
                var fileEnumerator = reader.ReadLines(filename).GetEnumerator();
                if (fileEnumerator.MoveNext())
                {   
                    var current = fileEnumerator.Current;
                    heap.Enqueue(filename, current);
                    fileToIterators[filename] = fileEnumerator;
                }
            }

            while (heap.TryDequeue(out var sourceFile, out var item))
            {
                yield return item;
                var iterator = fileToIterators[sourceFile];
                if (iterator.MoveNext())
                {
                    var nextCurrent = iterator.Current;
                    heap.Enqueue(sourceFile, nextCurrent);
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