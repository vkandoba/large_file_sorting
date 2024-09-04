using LargeFileSortingApp.SortingService;
using System.Text;

namespace LargeFileSortingApp.Utils;

public static class EnumerableExtension
{
    public static IEnumerable<LineItem[]> ChunksBySize(this IEnumerable<LineItem> lines, int chunkSizeB)
    {
        //TODO: check that size_bytes greater than 0
        IList<LineItem> current = new List<LineItem>(chunkSizeB / (2 * 1024));

        long currentSize = 0;
        foreach (var item in lines)
        {
            var size = Encoding.UTF8.GetByteCount(item.Line) + 1; // TODO: not efficient way to determinate size of string. May be replaced to mean size of file
            currentSize += size;
            current.Add(item);

            if (currentSize > chunkSizeB)
            {
                yield return current.ToArray();
                current.Clear();
                currentSize = 0;
            }
        }
        
        if (current.Any())
            yield return current.ToArray();
    }

    public static T[] SortInMemory<T>(this IEnumerable<T> items)
    {
        var itemsInMemory = items.ToArray();
        Array.Sort(itemsInMemory);
        return itemsInMemory;        
    }
}