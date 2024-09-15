using System.Diagnostics;

namespace LargeFileSortingTests;

public static class Utils
{
    public static void MeasureTime(Action code, Action<long> handleMeasuredTime)
    {
        var asyncMeasure = MeasureTime(() =>
        {
            code();
            return Task.CompletedTask;
        }, handleMeasuredTime);
        asyncMeasure.GetAwaiter().GetResult();
    }
    
    public static async Task MeasureTime(Func<Task> code, Action<long> handleMeasuredTime)
    {
        var timer = Stopwatch.StartNew();
        try
        {
            await code();
        }
        finally
        {
            var totalExecTime = timer.ElapsedMilliseconds;
            handleMeasuredTime(totalExecTime);
        }
    }
    
}