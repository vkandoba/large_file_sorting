namespace LargeFileSortingApp.SortingService;

public interface ISortingService
{
    IEnumerable<LineItem> GetSortedLines();
}