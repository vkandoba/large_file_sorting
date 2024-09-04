using LargeFileSortingApp.FileIO;

namespace LargeFileSortingApp.LineSortingService;

public interface ILineSortingServiceFactory
{
    ILineSortingService CreateService(string filename);
} 

public class LineSortingServiceFactory : ILineSortingServiceFactory
{
    private const long InMemorySortingBorderSizeB = 512 * 1024 * 1024; // 512 MB

    private const int SortingChunkSizeB =  128 * 1024 * 1024; // 128 MB

    public ILineSortingService CreateService(string filename)
    {
        var fileSizeB = new FileInfo(filename).Length;
        if (fileSizeB < InMemorySortingBorderSizeB)
        {
            var reader = new FileLineReader();
            var items = reader.ReadLines(filename); // not really execute here, it's just dependency
            return new InMemoryLineSortingService(items);
        }

        var chunkReader = new FileChunkLineReader(filename, Constants.FileOpBufferSizeB, SortingChunkSizeB);
        return new FileChunkLineSortingService(chunkReader, Constants.FileOpBufferSizeB);
    }
}