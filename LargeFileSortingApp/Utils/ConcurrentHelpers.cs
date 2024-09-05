using System.Collections.Concurrent;

namespace LargeFileSortingApp.Utils;

public static class ConcurrentHelpers
{
    public static Task[] StartConsumersForBlockedQueue<TItem>(
        BlockingCollection<TItem> blockingCollection,
        int consumersCount,
        Action<TItem> itemHandler)
    {
        return StartConsumersForBlockedQueue(blockingCollection, consumersCount, item =>
        {
            itemHandler(item);
            return 0;
        });
    }
    
    public static Task<TResult[]>[] StartConsumersForBlockedQueue<TItem, TResult>(
        BlockingCollection<TItem> blockingCollection, 
        int consumersCount, 
        Func<TItem, TResult> itemHandler)
    {
        var workerTasks = new List<Task<TResult[]>>();
        for (int i = 0; i < consumersCount; ++i)
        {
            var workerTask = Task.Factory.StartNew(() =>
            {
                List<TResult> results = new List<TResult>();
                do
                {
                    try
                    {
                        var item = blockingCollection.Take();
                        var handleResult = itemHandler(item);
                        results.Add(handleResult);
                    }
                    catch (InvalidOperationException)
                    {
                        if (!blockingCollection.IsAddingCompleted) // was raised from Take() because producers have done
                            throw;
                    }
                } while (!blockingCollection.IsAddingCompleted || blockingCollection.Count > 0);

                return results.ToArray();
            });
            workerTasks.Add(workerTask);
        }
        return workerTasks.ToArray();
    }
    
}