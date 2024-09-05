using System.Collections.Concurrent;
using System.Net.Http.Headers;
using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.Utils;

namespace LargeFileSortingApp.LineSortingService;

public class FileChunkLineSortingService : ILineSortingService, IDisposable
{
    private const int SortingWorkerCount = 6;
    
    private const string TempDirectoryNamePrefix = "tmp_dump_";

    private readonly IFileChunkLineReader _chunkLineReader;
    
    public FileChunkLineSortingService(IFileChunkLineReader chunkLineReader)
    {
        _chunkLineReader = chunkLineReader;
    }

    public IEnumerable<LineItem> GetSortedLines()
    {
        var chunks = _chunkLineReader.ReadChunks();

        var tempDir = FileHelpers.CreateTempDirectory(TempDirectoryNamePrefix);
        try
        {
            var chunkFiles = SortAndDumpToFiles(chunks, tempDir);
            Console.WriteLine("dump done"); 
            foreach (var item in MergeFiles(chunkFiles))
                yield return item;
        }
        finally
        {
            FileHelpers.CleanupDirectoryWithContent(tempDir);
        }
    }

    private string[] SortAndDumpToFiles(IEnumerable<LineItem[]> chunks, string dumpDir)
    {
        var chunkBlockedQueue = new BlockingCollection<LineItem[]>(SortingWorkerCount);
        var sortedChunkBlockedQueue = new BlockingCollection<LineItem[]>(SortingWorkerCount);
        
        Task.Factory.StartNew(() =>
        {
            try
            {
                foreach (var chunk in chunks)
                    chunkBlockedQueue.Add(chunk);
            }
            finally
            {
                chunkBlockedQueue.CompleteAdding();
            }
        });
    
        var sortWorkerTasks = new List<Task>();
        for (int i = 0; i < SortingWorkerCount; ++i)
        {
            var sortWorkerTask = Task.Factory.StartNew(() =>
            {
                do
                {
                    try
                    {
                        var chunkToSort = chunkBlockedQueue.Take();
                        var sortedChunk = chunkToSort.SortInMemory();
                        sortedChunkBlockedQueue.Add(sortedChunk);
                    }
                    catch (InvalidOperationException)
                    {
                        if (!chunkBlockedQueue.IsAddingCompleted) // was raised from take because read was done
                            throw;
                    }
                } while (!chunkBlockedQueue.IsAddingCompleted || chunkBlockedQueue.Count > 0);
            });
            sortWorkerTasks.Add(sortWorkerTask);
        }

        var dumpToFileWorkerTask = Task.Factory.StartNew<string[]>(() =>
        {
            var files = new List<string>();
            do
            {
                var sortedChunk = sortedChunkBlockedQueue.Take();
                var dumpFile = FileHelpers.GenerateUniqueFileName(dumpDir);
                FileHelpers.WriteLineItems(dumpFile, sortedChunk);
                Console.WriteLine("write chunk");                            
                files.Add(dumpFile);
            } while (!sortedChunkBlockedQueue.IsAddingCompleted || sortedChunkBlockedQueue.Count > 0);
            
            Console.WriteLine($"Write Task done");
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
                var fileEnumerator = FileHelpers.ReadLineItems(filename).GetEnumerator();
                if (fileEnumerator.MoveNext())
                {   
                    heap.Enqueue(filename, fileEnumerator.Current);
                    fileToIterators[filename] = fileEnumerator;
                }
            }

            while (heap.TryDequeue(out var sourceFile, out var item))
            {
                yield return item;

                var iterator = fileToIterators[sourceFile];
                if (iterator.MoveNext())
                    heap.Enqueue(sourceFile, iterator.Current);
            }
        }
        finally
        {
            foreach (var fileIterators in fileToIterators.Values)
                fileIterators.Dispose();
        }
    }

    public void Dispose()
    {
        _chunkLineReader.Dispose();
    }
}