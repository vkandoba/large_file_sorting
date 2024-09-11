namespace LargeFileSortingApp.FileIO;

public interface IFileChunkLineReader : IDisposable
{
    IAsyncEnumerable<LineItem[]> ReadChunks();
}