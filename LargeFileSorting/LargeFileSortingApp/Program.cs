// See https://aka.ms/new-console-template for more information
using System.IO;
using System.IO.Compression;

using LargeFileSorting;

var service = new FileSortingService();

var inFile = "/Users/vkandoba/data/default_test.txt";
var lines = File.ReadLines(inFile);

var result = service.Do(lines);

File.AppendAllLines("out.txt", result);

var firstResult = File.ReadLines("out.txt").Take(10);

foreach (var line in firstResult)
{
    Console.WriteLine(line);
}

