// See https://aka.ms/new-console-template for more information
using System.IO;
using System.IO.Compression;

using LargeFileSorting;

var service = new FileSortingService();

var lines = File.ReadLines("in.txt");

var result = service.Do(lines);

foreach (var line in result)
{
    Console.WriteLine(line);
}