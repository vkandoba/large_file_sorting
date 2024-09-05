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

    public static string CreateTempDirectory(string prefix)
    {
        string? name;
        do
        {
            var uniqueSuffix = Guid.NewGuid().ToString();
            name = prefix + uniqueSuffix;
        } while (Directory.Exists(name));

        Directory.CreateDirectory(name);
        return name;
    }

    public static void CleanupDirectoryWithContent(string name)
    {
        Directory.Delete(name, true);
    }

    public static string GenerateUniqueFileName(string dir)
    {
        string filename;
        do
        {
            var unique = Guid.NewGuid().ToString();
            filename = Path.Combine(dir, unique);
        } while (File.Exists(filename));

        return filename;
    }
}