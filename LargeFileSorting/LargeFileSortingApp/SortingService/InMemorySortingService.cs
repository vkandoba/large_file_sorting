namespace LargeFileSortingApp.SortingService;

public class InMemorySortingService : ISortingService
{
    public IEnumerable<LinePair> Sort(IEnumerable<LinePair> lines)
    {
        var linePairsInMemory = lines.ToArray();
        Array.Sort(linePairsInMemory);
        return linePairsInMemory;
    }
}