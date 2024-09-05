using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.LineSortingService;

// TODO: handle exceptions

var inputFile = args.Length > 0 ? args[0] : "in.txt";
var outputFile = args.Length > 1 ? args[1] : "out.txt";

#if DEBUG
    Console.WriteLine($"Input: {inputFile}\nOutput: {outputFile}"); 
    var totalWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

//TODO: check that input exists

var factory = new LineSortingServiceFactory();
var service = factory.CreateService(inputFile);
try
{
    var sortedLines = service.GetSortedLines();
    FileHelpers.WriteLineItems(outputFile, sortedLines);
}
finally
{
    if (service is IDisposable disposableService)
        disposableService.Dispose();
}

#if DEBUG
    var totalMs = totalWatch.ElapsedMilliseconds;
    Console.WriteLine($"Done. Internal measured time: {totalMs / 1000.0:N2} sec.\n");
#endif