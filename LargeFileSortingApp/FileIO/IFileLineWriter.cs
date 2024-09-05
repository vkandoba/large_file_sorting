namespace LargeFileSortingApp.FileIO;

public interface IFileLineWriter
{
    void WriteLines(string file, IEnumerable<LineItem> items);
}