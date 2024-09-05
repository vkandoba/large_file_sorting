namespace LargeFileSortingApp.LineSortingService;

public interface ILineSortingService
{
    IEnumerable<LineItem> GetSortedLines();
}