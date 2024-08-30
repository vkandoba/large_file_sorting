// See https://aka.ms/new-console-template for more information
using System.IO;
using System.IO.Compression;

using LargeFileSorting;

var service = new FileSortingService();

var lines = File.ReadLines("in.txt");

var result = service.Do(lines);

File.AppendAllLines("out.txt", result);

var firstResult = File.ReadAllLines("out.txt").Take(10);

foreach (var line in firstResult)
{
    Console.WriteLine(line);
}

