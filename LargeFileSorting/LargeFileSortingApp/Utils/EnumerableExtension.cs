using LargeFileSortingApp.SortingService;
using System.Text;

namespace LargeFileSortingApp.Utils;

public static class EnumerableExtension
{
    public static IEnumerable<LinePair[]> ChunksBySize(this IEnumerable<LinePair> lines, int chunkSizeB)
    {
        //TODO: check that size_bytes greater than 0
        IList<LinePair> current = new List<LinePair>(chunkSizeB / (2 * 1024));

        long currentSize = 0;
        foreach (var line in lines)
        {
            // TODO: not effient way to determinate size of string. May be replaced to mean size of file
            var strSize = Encoding.UTF8.GetByteCount(line.String);
            var numberSize = Encoding.UTF8.GetByteCount(line.Number);
            currentSize += strSize + numberSize;
            current.Add(line);

            if (currentSize > chunkSizeB)
            {
                // Console.WriteLine($"generate chunk of {current.Count} lines and {currentSize:n0} size");
                yield return current.ToArray();
                current.Clear();
                currentSize = 0;
            }
        }
    }
}