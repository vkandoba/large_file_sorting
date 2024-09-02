namespace LargeFileSortingApp.FileIO;

public interface ILineItemReader
{
    IEnumerable<LineItemWithMeta> ReadLines(string file);
}