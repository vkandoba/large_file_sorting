using LargeFileSortingApp.Utils;

namespace LargeFileSortingApp.LineSortingService;

public class InMemoryLineSortingService : ILineSortingService
{
    private readonly IEnumerable<LineItem> _itemsToSort;

    public InMemoryLineSortingService(IEnumerable<LineItem> itemsToSort)
    {
        _itemsToSort = itemsToSort;
    }

    public IEnumerable<LineItem> GetSortedLines() => _itemsToSort.SortInMemory();
}