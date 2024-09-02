using System.Text;

namespace LargeFileSortingApp.FileIO;

public class LineItemReader : ILineItemReader
{
    const int BufferSize =  64 * 1024; // ~64 KB
    
    public IEnumerable<LineItemWithMeta> ReadLines(string file)
    {
        var fileName = String.Intern(file);
        using var stream = File.OpenRead(file);
        using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: BufferSize);
        long pos = 0;
        while (reader.ReadLine() is { } line)
        {
            var item = LineItem.Parse(line);
            var size = (ushort)(stream.Position - pos); 
            pos = stream.Position;
            yield return new LineItemWithMeta{Item = item, Source = fileName, Size = size};
        }
    }
}