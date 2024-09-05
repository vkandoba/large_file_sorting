namespace LargeFileSortingApp.Utils;

public static class EnumerableExtension
{
    public static T[] SortInMemory<T>(this IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        
        var itemsInMemory = items.ToArray();
        Array.Sort(itemsInMemory);
        return itemsInMemory;        
    }
}