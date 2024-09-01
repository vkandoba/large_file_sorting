namespace LargeFileSortingApp.SortingService;

public interface ISortingService
{
    IEnumerable<LineItem> Sort(IEnumerable<LineItem> lines);
}