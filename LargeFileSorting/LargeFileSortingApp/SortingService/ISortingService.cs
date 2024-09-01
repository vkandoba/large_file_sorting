namespace LargeFileSortingApp.SortingService;

public interface ISortingService
{
    IEnumerable<LinePair> Sort(IEnumerable<LinePair> lines);
}