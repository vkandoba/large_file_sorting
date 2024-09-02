using LargeGenerateApp;

var totalWatch = System.Diagnostics.Stopwatch.StartNew();

var defaultConfigFileName = "default_config.json";
var configFileName = args.Length > 0 ? args[0] : defaultConfigFileName;
var config = GenerateSettings.ReadFromFile(configFileName);

var fileName = args.Length > 1 ? args[1] : config.File.DefaultName;

Console.WriteLine($"Start generating");
Console.WriteLine($"Config: {configFileName} | Seed: {config.RandomSeed} | File size: {config.File.MinSizeMb} Mb");

if (File.Exists(fileName))
{
    File.Delete(fileName);
}

var lines = new GenerateService().MakeLines(config);
File.AppendAllLines(fileName, lines);

var execMs = totalWatch.ElapsedMilliseconds;
Console.WriteLine($"Done | Time: {execMs / 1000.0:N2} sec.");
Console.WriteLine($"File: {fileName}");
Console.WriteLine($"Actual size: {new FileInfo(fileName).Length / (1024.0 * 1024.0):n2} Mb");

