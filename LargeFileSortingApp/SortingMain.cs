using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.LineSortingService;

if (args.Length < 1)
{
    PrintUsage();
    return;
}

var inputFile = args[0];
var outputFile = args.Length > 1 ? args[1] : "out.txt";

#if DEBUG
    Console.WriteLine($"Input: {inputFile}\nOutput: {outputFile}"); 
    var totalWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

if (!File.Exists(inputFile))
{
    Console.WriteLine($"Input file {inputFile} not found");
    return;
}

var outputDirName = Path.GetDirectoryName(outputFile);

if (!Directory.Exists(outputDirName) && !string.IsNullOrEmpty(outputDirName))
    Directory.CreateDirectory(outputDirName);

var factory = new LineSortingServiceFactory();
var service = factory.CreateService(inputFile);
try
{
    var sortedLines = service.GetSortedLines();
    FileHelpers.WriteLineItems(outputFile, sortedLines);
}
catch (Exception ex)
{
    Console.WriteLine($"File sorting app was failed: {ex.Message}");
    
    #if DEBUG
        throw;
    #endif
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

void PrintUsage()
{
    const string usage = @"
    usage: LargeFileSortingApp input_file [output_file] 
        input_file:  a file, that contains the lines to be sorted
        output_file: a output file with sorted lines. By default, it's ""out.txt""";
    
    Console.WriteLine(usage);  
}