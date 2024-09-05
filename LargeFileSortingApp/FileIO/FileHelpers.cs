using System.Text;

namespace LargeFileSortingApp.FileIO;

public static class FileHelpers
{
    public static IEnumerable<LineItem> ReadLineItems(string file, int bufferSize = Constants.FileOpBufferSizeB)
    {
        using var stream = File.OpenRead(file);
        using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: bufferSize);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return LineItem.Parse(line);
        }
    }
    
    public static void WriteLineItems(string file, IEnumerable<LineItem> items, int bufferSize = Constants.FileOpBufferSizeB)
    {
        using var stream = File.OpenWrite(file);
        using var writer = new StreamWriter(stream, Encoding.UTF8, bufferSize);
        foreach (var item in items)
        {
            writer.WriteLine(item.Line);
        }
    }
}