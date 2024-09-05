using LargeGenerateApp;

#if DEBUG
    var totalWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

var defaultConfigFileName = "configs/default_config.json";
var configFileName = args.Length > 0 ? args[0] : defaultConfigFileName;
var config = GenerateSettings.ReadFromFile(configFileName);

var fileName = args.Length > 1 ? args[1] : Path.GetFileNameWithoutExtension(configFileName);
var dirName = Path.GetDirectoryName(fileName);

if (!Directory.Exists(dirName) && !string.IsNullOrEmpty(dirName))
    Directory.CreateDirectory(dirName);

Console.WriteLine($"Config: {configFileName} | Seed: {config.RandomSeed} | " +
                  $"File size: {config.MinSizeMb} Mb | ");

var rnd = new Random(config.RandomSeed);
var service = new GenerateService(rnd, config.Generated.NumberPart, config.Generated.TextPartSize, config.Duplicated);
var lines=service.MakeRandomLines(config.MinSizeMb);

File.WriteAllLines(fileName, lines);

#if DEBUG
    var execMs = totalWatch.ElapsedMilliseconds;
    Console.WriteLine($"Generating time: {execMs / 1000.0:N2} sec.");
    Console.WriteLine($"Actual file size: {new FileInfo(fileName).Length / (1024.0 * 1024.0):n2} Mb");
#endif
    
Console.WriteLine(fileName);

