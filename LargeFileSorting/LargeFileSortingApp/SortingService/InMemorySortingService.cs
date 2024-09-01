namespace LargeFileSortingApp.SortingService;

public class InMemorySortingService : ISortingService
{
    public IEnumerable<LineItem> Sort(IEnumerable<LineItem> lines)
    {
        var linePairsInMemory = lines.ToArray();
        Array.Sort(linePairsInMemory);
        return linePairsInMemory;
    }
}