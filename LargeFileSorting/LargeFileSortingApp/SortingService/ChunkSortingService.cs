using System.Text;
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
    private const int WriteBufferSize =  64 * 1024; // ~64 KB

    private InMemorySortingService inMemotySorting = new InMemorySortingService();

    private const string TempFolder = "tmp_dump";

    public IEnumerable<LineItem> Sort(IEnumerable<LineItem> lines)
    {
        var chunks = lines.ChunksBySize(ChunkSize);
        var files = new List<string>();

        if (!Directory.Exists(TempFolder))
            Directory.CreateDirectory(TempFolder);
        
        Console.WriteLine(Path.GetFullPath(TempFolder));
        foreach (var chunk in chunks)
        {
            var uniqueName = Guid.NewGuid().ToString(); // TODO: handle collision
            var sortedChunk = inMemotySorting.Sort(chunk);
            var dumpFile = Path.Combine(TempFolder, uniqueName);
            using var dumpStream = File.OpenWrite(dumpFile);
            using var dumpWriter = new StreamWriter(dumpStream, Encoding.UTF8, WriteBufferSize);
            foreach (var item in sortedChunk)
            {
                dumpWriter.WriteLine(item.Line);
            }

            files.Add(dumpFile);
        }
        
        var result = MergeFiles(files.ToArray());
        foreach (var item in result)
        {
            yield return item;
        }
        
        foreach (var file in files)
        {
            File.Delete(file);
        }

        //TODO: race condition on double run at the same time
        if (!Directory.EnumerateFileSystemEntries(TempFolder).Any())
        {
            Directory.Delete(TempFolder);
        }
    }
    
    private IEnumerable<LineItem> MergeFiles(string[] files)
    {
        var fileToIterators = new Dictionary<string, IEnumerator<string>>();
        var heap = new PriorityQueue<LinePairFromDump, LineItem>();
        foreach (var file in files)
        {
            var fileEnumerator = File.ReadLines(file).GetEnumerator();
            if (fileEnumerator.MoveNext())
            {
                var current = LineItem.Parse(fileEnumerator.Current);
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
                var nextCurrent = LineItem.Parse(iterator.Current);
                var nextItem = new LinePairFromDump { file = item.file, LineItem = nextCurrent };
                heap.Enqueue(nextItem, nextCurrent);
            }
        }
    }
}