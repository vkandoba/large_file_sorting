// See https://aka.ms/new-console-template for more information
using System.IO;
using System.IO.Compression;
using LargeFileSortingApp.SortingService;

var totalWatch = System.Diagnostics.Stopwatch.StartNew();

var service = new ChunkSortingService();

var inFile = "/Users/vkandoba/data/default_test.txt";
var textLines = File.ReadLines(inFile);
var lines = textLines.Select(LinePair.Parse);
var sortedLines = service.Sort(lines);
var result = sortedLines.Select(l => l.MakeString());

var outputFile = "out.txt";
if (File.Exists(outputFile))
{
    File.Delete(outputFile);
}
File.AppendAllLines("out.txt", result);

var total_ms = totalWatch.ElapsedMilliseconds;

Console.WriteLine($"total exec time: {total_ms / 1000:N2} sec.\n");

var firstResult = File.ReadLines("out.txt").Take(10);


foreach (var line in firstResult)
{
    Console.WriteLine(line);
}

