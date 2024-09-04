namespace LargeFileSortingApp.FileIO;

public interface IFileLineItemWriter
{
    void WriteLines(string file, IEnumerable<LineItem> items);
}