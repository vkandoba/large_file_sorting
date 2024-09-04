namespace LargeFileSortingApp.FileIO;

public interface IFileChunkLineReader : IDisposable
{
    IEnumerable<LineItem[]> ReadChunks();
}