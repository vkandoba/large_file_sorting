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
        foreach (var item in lines)
        {
            var size = Encoding.UTF8.GetByteCount(item.Line); // TODO: not efficient way to determinate size of string. May be replaced to mean size of file
            currentSize += size;
            current.Add(item);

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