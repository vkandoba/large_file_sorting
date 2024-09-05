using LargeGenerateApp;

#if DEBUG
    var totalWatch = System.Diagnostics.Stopwatch.StartNew();
#endif
    
if (args.Length < 2)
{
    const string usage = @"
    usage: LargeGenerateApp config filename 
        config:  file with generating settings 
        filename: file to be generated;
        
        Config example: 
        {
          ""RandomSeed"": 42,
          ""MinSizeMb"": 0.05,
          ""Duplicated"":
          { 
            ""Rate"": 0.3,
            ""Line"": ""648.мћhњнвxrтaafaдzmуџалcmrpаgџrкdхaцфbцwжvољvwepтuљsrnhdђ""
          },
          ""Generated"": 
          {
            ""NumberPart"": {""Min"":  1000000, ""Max"":  922337203685477580},
            ""TextPartSize"": {""Min"":  30, ""Max"":  250}
          }
        }";
    
    Console.WriteLine(usage);
    return;
}

var configFileName = args[0];
var config = GenerateSettings.ReadFromFile(configFileName);

var fileName = args[1];
var dirName = Path.GetDirectoryName(fileName);

if (!Directory.Exists(dirName) && !string.IsNullOrEmpty(dirName))
    Directory.CreateDirectory(dirName);

Console.WriteLine($"Config: {configFileName} | Seed: {config.RandomSeed} | " +
                  $"File size: {config.MinSizeMb} Mb | ");
try
{
    var rnd = new Random(config.RandomSeed);
    var service = new GenerateService(rnd, config.Generated?.NumberPart, config.Generated?.TextPartSize,
        config.Duplicated);
    var lines = service.MakeRandomLines(config.MinSizeMb);

    File.WriteAllLines(fileName, lines);
}
catch (Exception ex)
{
    Console.WriteLine($"Fail to generate a file: {ex.Message}");
    
    #if DEBUG
        Console.WriteLine($"{ex.GetType()}");
        Console.WriteLine($"{ex.StackTrace}");
    #endif
}

#if DEBUG
    var execMs = totalWatch.ElapsedMilliseconds;
    Console.WriteLine($"Generating time: {execMs / 1000.0:N2} sec.");
    Console.WriteLine($"Actual file size: {new FileInfo(fileName).Length / (1024.0 * 1024.0):n2} Mb");
#endif
    
Console.WriteLine(fileName);

