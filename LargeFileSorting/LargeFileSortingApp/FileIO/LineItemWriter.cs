using System.Text;

namespace LargeFileSortingApp.FileIO;

public class LineItemWriter : ILineItemWriter
{
    private const int BufferSize =  64 * 1024; // ~64 KB

    public void Write(string file, IEnumerable<LineItem> items)
    {
        using var stream = File.OpenWrite(file);
        using var writer = new StreamWriter(stream, Encoding.UTF8, BufferSize);
        foreach (var item in items)
        {
            writer.WriteLine(item.Line);
        }
    }
}