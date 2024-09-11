using System.Collections.Concurrent;
using System.Threading.Channels;
using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.Utils;

namespace LargeFileSortingApp.LineSortingService;

public class FileChunkLineSortingService : ILineSortingService, IDisposable
{
    private const int SortingWorkerCount = 6;
    
    private const int DumpWorkerCount = 4;
    
    private const string TempDirectoryNamePrefix = "tmp_dump_";

    private readonly IFileChunkLineReader _chunkLineReader;
    
    public FileChunkLineSortingService(IFileChunkLineReader chunkLineReader)
    {
        _chunkLineReader = chunkLineReader;
    }

    public async IAsyncEnumerable<LineItem> GetSortedLines()
    {
        var chunks = _chunkLineReader.ReadChunks();

        var tempDir = FileHelpers.CreateTempDirectory(TempDirectoryNamePrefix);
        try
        {
            var chunkFiles = SortAndDumpToFiles(chunks, tempDir);
            await foreach (var item in MergeFiles(chunkFiles))
                yield return item;
        }
        finally
        {
            FileHelpers.CleanupDirectoryWithContent(tempDir);
        }
    }

    private string[] SortAndDumpToFiles(IAsyncEnumerable<LineItem[]> chunks, string dumpDir)
    {
        var chunkBlockedQueue = Channel.CreateBounded<LineItem[]>(SortingWorkerCount);
        var sortedChunkBlockedQueue = Channel.CreateBounded<LineItem[]>(SortingWorkerCount);
        
        ProduceChunksToBlockedQueue(chunks, chunkBlockedQueue);

        var sortWorkerTasks = ConcurrentHelpers.StartConsumersForBlockedQueue(
            chunkBlockedQueue, SortingWorkerCount, async chunk =>
        {
            var sortedChunk = chunk.SortInMemory();
            await sortedChunkBlockedQueue.Writer.WriteAsync(sortedChunk);
        });

        var dumpWorkerTasks = ConcurrentHelpers.StartConsumersForBlockedQueue(
            sortedChunkBlockedQueue, DumpWorkerCount, async sortedChunk =>
            {
                var dumpFile = FileHelpers.GenerateUniqueFileName(dumpDir);
                await FileHelpers.WriteLineItems(dumpFile, sortedChunk.ToAsyncEnumerable());
                return dumpFile;
            }
        );

        Task.WaitAll(sortWorkerTasks.ToArray());
        sortedChunkBlockedQueue.Writer.Complete();
        
        var files = new List<string>();
        foreach (var dumpTask in dumpWorkerTasks)
        {
            var dumpFiles = dumpTask.Result;
            files.AddRange(dumpFiles);
        }
        return files.ToArray();
    }

    private async Task ProduceChunksToBlockedQueue(IAsyncEnumerable<LineItem[]> chunks, Channel<LineItem[]> chunkQueue)
    {
        try
        {
            await foreach (var chunk in chunks)
            {
                await chunkQueue.Writer.WriteAsync(chunk);
            }
        }
        finally
        {
            chunkQueue.Writer.Complete();
        }        
    }
    
    private async IAsyncEnumerable<LineItem> MergeFiles(string[] filenames)
    {
        var fileToIterators = new Dictionary<string, IAsyncEnumerator<LineItem>>();
        try
        {
            var heap = new PriorityQueue<string, LineItem>();
            foreach (var filename in filenames)
            {
                string.Intern(filename);
                var fileEnumerator = FileHelpers.ReadLineItems(filename).GetAsyncEnumerator();
                if (await fileEnumerator.MoveNextAsync())
                {   
                    heap.Enqueue(filename, fileEnumerator.Current);
                    fileToIterators[filename] = fileEnumerator;
                }
            }

            while (heap.TryDequeue(out var sourceFile, out var item))
            {
                yield return item;

                var iterator = fileToIterators[sourceFile];
                if (await iterator.MoveNextAsync())
                    heap.Enqueue(sourceFile, iterator.Current);
            }
        }
        finally
        {
            foreach (var fileIterators in fileToIterators.Values)
                fileIterators.DisposeAsync();
        }
    }
    
    public void Dispose()
    {
        _chunkLineReader.Dispose();
    }
}