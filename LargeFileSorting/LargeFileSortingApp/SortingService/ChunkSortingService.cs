using System.Collections.Concurrent;
using System.Text;
using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.Utils;
using Microsoft.VisualBasic.CompilerServices;

namespace LargeFileSortingApp.SortingService;

public class LinePairFromDump
{
    public string file { get; set; }
    public LineItem LineItem { get; set; }
}

public class ChunkSortingService : ISortingService
{
    private const int ChunkSize =  128 * 1024 * 1024; // ~128 MB
    
    private const string TempFolder = "tmp_dump";

    public IEnumerable<LineItem> Sort(IEnumerable<LineItem> lines)
    {
        var chunks = lines.ChunksBySize(ChunkSize);
        
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
        if (!Directory.EnumerateFileSystemEntries(TempFolder).Any())
        {
            Directory.Delete(TempFolder);
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
            var dumpFiles = new List<string>();
            
            if (!Directory.Exists(TempFolder))
                Directory.CreateDirectory(TempFolder);
            
            var itemWriter = new LineItemWriter();
            do
            {
                var sortedChunk = sortedChunkBlockedQueue.Take();
                var uniqueName = Guid.NewGuid().ToString(); // TODO: handle collision
                var dumpFile = Path.Combine(TempFolder, uniqueName);
                itemWriter.Write(dumpFile, sortedChunk);
                dumpFiles.Add(dumpFile);
            } while (!sortedChunkBlockedQueue.IsAddingCompleted || sortedChunkBlockedQueue.Count > 0);

            return dumpFiles.ToArray();
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
            var heap = new PriorityQueue<LinePairFromDump, LineItem>();
            foreach (var file in files)
            {
                string.Intern(file);
                var reader = new FileLineItemReader();
                var fileEnumerator = reader.ReadLines(file).GetEnumerator();
                if (fileEnumerator.MoveNext())
                {   
                    var current = fileEnumerator.Current;
                    var item = new LinePairFromDump { file = file, LineItem = current };
                    heap.Enqueue(item, current);
                    fileToIterators[file] = fileEnumerator;
                }
            }

            while (heap.TryDequeue(out var item, out var pair))
            {
                yield return pair;
                var iterator = fileToIterators[item.file];
                if (iterator.MoveNext())
                {
                    var nextCurrent = iterator.Current;
                    var nextItem = new LinePairFromDump { file = item.file, LineItem = nextCurrent };
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
}