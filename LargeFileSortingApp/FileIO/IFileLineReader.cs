namespace LargeFileSortingApp.FileIO;

public interface IFileLineReader
{
    IEnumerable<LineItem> ReadLines(string file);
}