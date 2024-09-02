namespace LargeFileSortingApp.FileIO;

public interface ILineItemReader
{
    IEnumerable<LineItem> ReadLines(string file);
}