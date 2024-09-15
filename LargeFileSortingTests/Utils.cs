using System.Diagnostics;

namespace LargeFileSortingTests;

public static class Utils
{
    public static void MeasureTime(Action code, Action<long> handleMeasuredTime)
    {
        var timer = Stopwatch.StartNew();
        ;
        try
        {
            code();
        }
        finally
        {
            var totalExecTime = timer.ElapsedMilliseconds;
            handleMeasuredTime(totalExecTime);
        }
    }
}