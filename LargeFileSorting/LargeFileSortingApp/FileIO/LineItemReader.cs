using System.Text;

namespace LargeFileSortingApp.FileIO;

public class LineItemReader : ILineItemReader
{
    const int BufferSize =  64 * 1024; // ~64 KB
    
    public IEnumerable<LineItem> ReadLines(string file)
    {
        using var stream = File.OpenRead(file);
        using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: BufferSize);
        while (reader.ReadLine() is { } line)
        {
            yield return LineItem.Parse(line);
        }
    }
}