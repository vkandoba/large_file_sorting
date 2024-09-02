// See https://aka.ms/new-console-template for more information

using System.Text;
using LargeFileSortingApp;
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

var textLines = File.ReadLines(inputFile);
var lines = textLines.Select(LineItem.Parse);
var sortedLines = service.Sort(lines);
var result = sortedLines.Select(l => l.Line);

if (File.Exists(outputFile))
{
    File.Delete(outputFile);
}
File.AppendAllLines(outputFile, result);
var total_ms = totalWatch.ElapsedMilliseconds;

const int ReadBufferSize =  64 * 1024; // ~64 KB

IEnumerable<LineItem> ReadItems(string file)
{
    using var stream = File.OpenRead(file);
    using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: ReadBufferSize);
    while (reader.ReadLine() is { } line)
    {
        yield return LineItem.Parse(line);
    }
}

#if DEBUG
    Console.WriteLine($"Done. Exec time: {total_ms / 1000.0:N2} sec.\n");
#endif

var firstResult = File.ReadLines(outputFile).Take(10);

foreach (var line in firstResult)
{
    Console.WriteLine(line);
}
