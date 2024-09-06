using System.Collections.Concurrent;

namespace LargeFileSortingApp.Utils;

public static class ConcurrentHelpers
{
    public static Task[] StartConsumersForBlockedQueue<TItem>(
        BlockingCollection<TItem> blockingCollection, 
        int consumersCount, 
        Action<TItem> itemHandler)
    {
        var workerTasks = new List<Task>();
        for (int i = 0; i < consumersCount; ++i)
        {
            var workerTask = Task.Factory.StartNew(() =>
            {
                do
                {
                    try
                    {
                        var item = blockingCollection.Take();
                        itemHandler(item);
                    }
                    catch (InvalidOperationException)
                    {
                        if (!blockingCollection.IsAddingCompleted) // was raised from Take() because producers have done
                            throw;
                    }
                } while (!blockingCollection.IsAddingCompleted || blockingCollection.Count > 0);

            });
            workerTasks.Add(workerTask);
        }
        return workerTasks.ToArray();
    }
    
}