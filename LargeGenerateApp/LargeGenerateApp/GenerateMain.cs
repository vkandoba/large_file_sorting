// See https://aka.ms/new-console-template for more information

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

var totalWatch = System.Diagnostics.Stopwatch.StartNew();

var config_file_name = "default_config.json";
var config = GenerateSettings.ReadFromFile(config_file_name);
Console.WriteLine($"Start generating");
Console.WriteLine($"Config: {config_file_name} | Seed: {config.RandomSeed} | File size: {config.File.MinSizeMb} Mb");

var fileName = config.File.Path;
var rnd = new Random(config.RandomSeed);
if (File.Exists(fileName))
{
    File.Delete(fileName);
}

var lines = GenerateLines(config);
File.AppendAllLines(fileName, lines);

var total_ms = totalWatch.ElapsedMilliseconds;

var fileAttr = new FileInfo(fileName);
Console.WriteLine($"Done | Time: {total_ms / 1000.0:N2} sec.");
Console.WriteLine($"File: {fileName}");
Console.WriteLine($"Actual size: {fileAttr.Length / (1024.0 * 1024.0):n2} Mb");

IEnumerable<string> GenerateLines(GenerateSettings settings)
{
    ulong sizeInBytes = (ulong)(settings.File.MinSizeMb * 1024 * 1024); // TODO: move to constant
    ulong actualSizeInBytes = 0;
    
    while (actualSizeInBytes < sizeInBytes)
    {
        var numberRaw = (ulong) rnd.NextInt64(settings.Line.Number.Min, settings.Line.Number.Max);
        var numberStr = numberRaw.ToString();
        
        var textLen = rnd.Next((int)settings.Line.TextSize.Min, (int)settings.Line.TextSize.Max);
        var textRaw = new byte[textLen];
        rnd.NextBytes(textRaw);
        var text = System.Text.Encoding.ASCII.GetString(textRaw.Select(n => (byte)(97 + n % 25)).ToArray());
        
        actualSizeInBytes += (ulong) (numberStr.Length + textLen);
        yield return $"{numberStr}.{text}";
    }
}

[DataContract]
public class GenerateSettings
{
    [DataMember]
    public int RandomSeed { get; set; }
    
    [DataMember]
    public FileGenerateSettings File { get; set; }
 
    [DataMember]
    public LineGenerateSettings Line { get; set; }

    public static GenerateSettings ReadFromFile(string configFile)
    {
        var s = new DataContractJsonSerializer(typeof(GenerateSettings));
        
        using var fs = System.IO.File.OpenRead(configFile);
        var obj = s.ReadObject(fs) as GenerateSettings;
        if (obj == null)
        {
            throw new ArgumentException($"Failed to read app settings from {configFile}. Please, check it");
        }
        return obj;
    }
}

[DataContract]
public class FileGenerateSettings
{
    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public float MinSizeMb { get; set; }
}

[DataContract]
public class LineGenerateSettings
{
    [DataMember]
    public LinePartGenerateSettings Number { get; set; }

    [DataMember]
    public LinePartGenerateSettings TextSize { get; set; }
}

[DataContract]
public class LinePartGenerateSettings
{
    [DataMember]
    public long Min { get; set; }

    [DataMember]
    public long Max { get; set; }
}