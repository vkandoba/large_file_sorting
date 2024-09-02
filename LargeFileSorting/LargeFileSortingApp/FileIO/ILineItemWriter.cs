namespace LargeFileSortingApp.FileIO;

public interface ILineItemWriter
{
    void Write(string file, IEnumerable<LineItem> items);
}