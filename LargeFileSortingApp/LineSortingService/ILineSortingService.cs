namespace LargeFileSortingApp.LineSortingService;

public interface ILineSortingService
{
    IAsyncEnumerable<LineItem> GetSortedLines();
}