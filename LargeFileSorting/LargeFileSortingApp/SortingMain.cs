// See https://aka.ms/new-console-template for more information

using System.Text;
using LargeFileSortingApp;
using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.SortingService;

// TODO: handle exceptions
// TODO: profile memory for 1 GB

var inputFile = args.Length > 0 ? args[0] : "in.txt";
var outputFile = args.Length > 1 ? args[1] : "out.txt";

#if DEBUG
    Console.WriteLine($"Input: {inputFile}\nOutput: {outputFile}"); 
#endif

var totalWatch = System.Diagnostics.Stopwatch.StartNew();

var service = new ChunkSortingService();

var reader = new LineItemReader();
var lines = reader.ReadLines(inputFile);
var sortedLines = service.Sort(lines);

if (File.Exists(outputFile))
{
    File.Delete(outputFile);
}

var writer = new LineItemWriter();
writer.Write(outputFile, sortedLines);
var total_ms = totalWatch.ElapsedMilliseconds;

#if DEBUG
    Console.WriteLine($"Done. Exec time: {total_ms / 1000.0:N2} sec.\n");
#endif

var firstResult = File.ReadLines(outputFile).Take(10);

#if DEBUG
    foreach (var line in firstResult)
    {
        Console.WriteLine(line);
    }
    Console.WriteLine();
#endif
