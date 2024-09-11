using System.Collections.Concurrent;
using System.Threading.Channels;

namespace LargeFileSortingApp.Utils;

public static class ConcurrentHelpers
{
    public static Task[] StartConsumersForBlockedQueue<TItem>(
        Channel<TItem> blockingCollection,
        int consumersCount,
        Action<TItem> itemHandler)
    {
        return StartConsumersForBlockedQueue(blockingCollection, consumersCount, item =>
        {
            itemHandler(item);
            return Task.FromResult(0);
        });
    }
    
    public static Task<TResult[]>[] StartConsumersForBlockedQueue<TItem, TResult>(
        Channel<TItem> blockingCollection, 
        int consumersCount, 
        Func<TItem, Task<TResult>> itemHandler)
    {
        var workerTasks = new List<Task<TResult[]>>();
        for (int i = 0; i < consumersCount; ++i)
        {
            var workerTask = async () =>
            {
                List<TResult> results = new List<TResult>();
                while (await blockingCollection.Reader.WaitToReadAsync())
                {
                    if (blockingCollection.Reader.TryRead(out var item))
                    {
                        var handleResult = await itemHandler(item);
                        results.Add(handleResult);
                    }
                }

                return results.ToArray();
            };
            workerTasks.Add(workerTask());
        }
        return workerTasks.ToArray();
    }
}