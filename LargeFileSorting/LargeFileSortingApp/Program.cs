// See https://aka.ms/new-console-template for more information
using System.IO;
using System.IO.Compression;
using LargeFileSortingApp.SortingService;

var totalWatch = System.Diagnostics.Stopwatch.StartNew();

var service = new InMemorySortingService();

var inFile = "/Users/vkandoba/data/default_test.txt";
var textLines = File.ReadLines(inFile);
var lines = ParseLines(textLines);

// var chunks = lines.Chunk(10000);

var sortedLines = service.Sort(lines);
var result = sortedLines.Select(l => l.MakeString());

var outputFile = "out.txt";
if (File.Exists(outputFile))
{
    File.Delete(outputFile);
}
File.AppendAllLines("out.txt", result);

var total_ms = totalWatch.ElapsedMilliseconds;

Console.WriteLine($"total exec time: {total_ms:N0}\n");

var firstResult = File.ReadLines("out.txt").Take(10);



foreach (var line in firstResult)
{
    Console.WriteLine(line);
}

IEnumerable<LinePair> ParseLines(IEnumerable<string> lines)
{
    foreach (var line in lines)
    {
        var tokens = line.Split('.');
        yield return new LinePair { Number = tokens[0], String = tokens[1] };
    }
}
