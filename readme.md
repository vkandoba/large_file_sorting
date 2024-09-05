## Task

Sort lines from a large file, where each line is a "[Number]. [String]".

Ordering criteria: String part is compared first, if it matches then
Number.

For example:
```
For example:
415. Apple
30432. Something something something
1. Apple
32. Cherry is the best
2. Banana is yellow
```

Output should be:
```
1. Apple
415. Apple
2. Banana is yellow
32. Cherry is the best
30432. Something something something
```

## Build and run

By make and bash-like env:
````
make run [input_file] [output_file]
````

By dotnet directly:
````
dotnet publish LargeFileSortingApp/LargeFileSortingApp.csproj -o ./bin -c Release --framework net6.0
dotnet ./bin/LargeFileSortingApp.dll [input_file] [output_file]
````

## Generate test

By make and bash-like env:
````
make gen [config.json] [test_file]
````

By dotnet directly:
```
dotnet run --project LargeGenerateApp/ -- [config.json] [test_file]
````

The config example, that specifies file size:
```
{
  "RandomSeed": 42,
  "MinSizeMb": 0.05,
  "Duplicated":
  { 
    "Rate": 0.3,
    "Line": "648.мћhњнвxrтaafaдzmуџалcmrpаgџrкdхaцфbцwжvољvwepтuљsrnhdђ"
  },
  "Generated": 
  {
    "NumberPart": {"Min":  1000000, "Max":  922337203685477580},
    "TextPartSize": {"Min":  30, "Max":  250}
  }
}
```


[GeneratingMain](https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/LineSortingService/FileChunkLineSortingService.cs#L92)

[GenerateService.MakeRandomLines](https://github.com/vkandoba/large_file_sorting/blob/main/LargeGenerateApp/GenerateService.cs#L45)

## Solution

* Split file to chunks and in-memory sorting for each

[ReadChunks](https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/FileIO/FileChunkLineReader.cs#L26)

[SortInMemory](https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/Utils/EnumerableExtension.cs#L5)

* Dump chunks to files on a disk

[SortAndDumpToFiles](https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/LineSortingService/FileChunkLineSortingService.cs#L57)

* K-way merge files by a priority queue in memory

[MergeFiles](https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/LineSortingService/FileChunkLineSortingService.cs#L92)

### Optimizations

* Just in-memory sorting for files less than 512 MB

[SortingServiceFactory] (https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/LineSortingService/LineSortingServiceFactory.cs#L25)

* File r/w buffer size is 64 Kb

[FileIO.Constants] (https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/FileIO/Constants.cs#L5)

* Concurrent chunk sorting and writing

[SortAndDumpToFiles] (https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/LineSortingService/FileChunkLineSortingService.cs#L46)

[StartConsumersForBlockedQueue] (https://github.com/vkandoba/large_file_sorting/blob/main/LargeFileSortingApp/Utils/ConcurrentHelpers.cs#L19)
