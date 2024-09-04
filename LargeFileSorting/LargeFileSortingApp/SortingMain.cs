using LargeFileSortingApp.FileIO;
using LargeFileSortingApp.SortingService;

// TODO: handle exceptions

var inputFile = args.Length > 0 ? args[0] : "in.txt";
var outputFile = args.Length > 1 ? args[1] : "out.txt";

#if DEBUG
    Console.WriteLine($"Input: {inputFile}\nOutput: {outputFile}"); 
    var totalWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

using var chunkReader = new FileChunkLineReader(inputFile, 64 * 1024, 128 * 1024 * 1024);
var service = new ChunkSortingService(chunkReader);

var reader = new FileLineItemReader();
var lines = reader.ReadLines(inputFile);
var sortedLines = service.Sort(lines);

if (File.Exists(outputFile))
{
    File.Delete(outputFile);
}

var writer = new FileLineItemWriter();
writer.WriteLines(outputFile, sortedLines);

#if DEBUG
    var totalMs = totalWatch.ElapsedMilliseconds;
    Console.WriteLine($"Done. Internal measured time: {totalMs / 1000.0:N2} sec.\n");
#endif