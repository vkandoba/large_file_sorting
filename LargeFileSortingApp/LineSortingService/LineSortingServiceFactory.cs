using LargeFileSortingApp.FileIO;

namespace LargeFileSortingApp.LineSortingService;

public interface ILineSortingServiceFactory
{
    ILineSortingService CreateService(string filename);
} 

public class LineSortingServiceFactory : ILineSortingServiceFactory
{
    private const long InMemorySortingMaxFileSizeB = 512 * 1024 * 1024; // 512 MB

    private const long LargeChunkFileSizeB =  (long) 10 * 1024 * 1024 * 1024; // 10 GB

    private const int MediumSortingChunkSizeB =  128 * 1024 * 1024; // 128 MB

    private const int LargeSortingChunkSizeB =  512 * 1024 * 1024; // 512 MB

    
    public ILineSortingService CreateService(string filename)
    {
        var fileSizeB = new FileInfo(filename).Length;
        if (fileSizeB <= InMemorySortingMaxFileSizeB)
        {
            var reader = new FileLineReader();
            var items = reader.ReadLines(filename); // not really execute here, it's just dependency
            return new InMemoryLineSortingService(items);
        }

        var chunkSizeB = fileSizeB < LargeChunkFileSizeB ? MediumSortingChunkSizeB : LargeSortingChunkSizeB;
        var chunkReader = new FileChunkLineReader(filename, Constants.FileOpBufferSizeB, chunkSizeB);
        return new FileChunkLineSortingService(chunkReader, Constants.FileOpBufferSizeB);
    }
}