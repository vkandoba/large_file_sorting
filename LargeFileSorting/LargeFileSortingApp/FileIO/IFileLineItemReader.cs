namespace LargeFileSortingApp.FileIO;

public interface IFileLineItemReader
{
    IEnumerable<LineItem> ReadLines(string file);
}