using Microsoft.VisualBasic.CompilerServices;

namespace LargeFileSortingApp.SortingService;

public class LinePairFromDump
{
    public string file { get; set; }
    public LinePair LinePair { get; set; }
}

public class ChunkSortingService : ISortingService
{
    private const int ChunkSize = 10 * 64000; // ~250 MB, TODO: write method for spliting chunk by size

    private InMemorySortingService inMemotySorting = new InMemorySortingService();

    private const string TempFolder = "tmp_dump";

    public IEnumerable<LinePair> Sort(IEnumerable<LinePair> lines)
    {
        var chunks = lines.Chunk(ChunkSize);
        var files = new List<string>();

        if (!Directory.Exists(TempFolder))
            Directory.CreateDirectory(TempFolder);
        
        Console.WriteLine(Path.GetFullPath(TempFolder));
        foreach (var chunk in chunks)
        {
            var uniqueName = Guid.NewGuid().ToString(); // TODO: handle collision
            var sortedChunk = inMemotySorting.Sort(chunk);
            var dumpFile = Path.Combine(TempFolder, uniqueName);
            File.AppendAllLines(dumpFile, sortedChunk.Select(x => x.MakeString()));
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
    }
    
    private IEnumerable<LinePair> MergeFiles(string[] files)
    {
        var fileToIterators = new Dictionary<string, IEnumerator<string>>();
        var heap = new PriorityQueue<LinePairFromDump, LinePair>();
        foreach (var file in files)
        {
            var fileEnumerator = File.ReadLines(file).GetEnumerator();
            if (fileEnumerator.MoveNext())
            {
                var current = LinePair.Parse(fileEnumerator.Current);
                var item = new LinePairFromDump { file = file, LinePair = current };
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
                var nextCurrent = LinePair.Parse(iterator.Current);
                var nextItem = new LinePairFromDump { file = item.file, LinePair = nextCurrent };
                heap.Enqueue(nextItem, nextCurrent);
            }
        }
    }
}