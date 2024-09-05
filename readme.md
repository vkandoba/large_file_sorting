### Task

Sorting lines from a too large file, where each line is a "[Number]. [String]"

### Build and run

By make and bash-like env:
````
make run [input_file] [output_file]
````

By dotnet directly:
````
dotnet publish LargeFileSortingApp/LargeFileSortingApp.csproj -o ./bin -c Release --framework net6.0
dotnet ./bin/LargeFileSortingApp.dll [input_file] [output_file]
````

### Generate test

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

### Algorithm

1. Split to chunks and in-memory sorting 
2. Dump chunks to disk
3. k-Way Merge chunks
