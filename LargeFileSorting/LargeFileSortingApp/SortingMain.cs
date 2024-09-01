// See https://aka.ms/new-console-template for more information
using System.IO;
using System.IO.Compression;
using LargeFileSortingApp.SortingService;

// TODO: handle exceptions
// TODO: profile memory for 1 GB
var totalWatch = System.Diagnostics.Stopwatch.StartNew();

var service = new ChunkSortingService();

var inFile = "/Users/vkandoba/data/test_1_gb_30_250";
var textLines = File.ReadLines(inFile);
var lines = textLines.Select(LineItem.Parse);
var sortedLines = service.Sort(lines);
var result = sortedLines.Select(l => l.Line);

var outputFile = "out.txt";
if (File.Exists(outputFile))
{
    File.Delete(outputFile);
}
File.AppendAllLines(outputFile, result);

var total_ms = totalWatch.ElapsedMilliseconds;

Console.WriteLine($"total exec time: {total_ms / 1000.0:N2} sec.\n");

var firstResult = File.ReadLines("out.txt").Take(10);

foreach (var line in firstResult)
{
    Console.WriteLine(line);
}

