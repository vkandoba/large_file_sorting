using LargeFileSortingApp.SortingService;
using System.Text;

namespace LargeFileSortingApp.Utils;

public static class EnumerableExtension
{
    public static IEnumerable<IEnumerable<LinePair>> ChunksBySize(this IEnumerable<LinePair> lines, uint chunkSizeB)
    {
        //TODO: check that size_bytes greater than 0
        IList<LinePair> current = new List<LinePair>((int)(chunkSizeB / 1024));

        long currentSize = 0;
        foreach (var line in lines)
        {
            var strSize = Encoding.UTF8.GetByteCount(line.String);
            var numberSize = Encoding.UTF8.GetByteCount(line.String);
            currentSize += strSize + numberSize;
            current.Add(line);

            if (currentSize < chunkSizeB)
            {
                yield return current;
                current.Clear();
            }
        }
    }
}