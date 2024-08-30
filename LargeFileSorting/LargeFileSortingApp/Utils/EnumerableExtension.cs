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
            var strSize = line.Number.Length * 2;
            var numberSize = line.String.Length * 2;
            currentSize += strSize + numberSize;
            current.Add(line);

            if (currentSize < chunkSizeB)
            {
                yield return current.ToArray();
                current.Clear();
                currentSize = 0;
            }
        }
    }
}