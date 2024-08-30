// See https://aka.ms/new-console-template for more information

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

Console.WriteLine("Start generate");
var rnd = new Random();
    
var settings = GenerateSettings.ReadFromFile("default_config.json");
var fileName = settings.File.Path;
Console.WriteLine(fileName);
if (File.Exists(fileName))
{
    File.Delete(fileName);
}

var lines = GenerateLines(settings);
File.AppendAllLines(fileName, lines);

var fileAttr = new FileInfo(fileName);
Console.WriteLine($"{fileAttr.Length / 1024.0:n2} KB");

IEnumerable<string> GenerateLines(GenerateSettings settings)
{
    var sizeInBytes = settings.File.MinSizeMb * 1024 * 1024; // TODO: move to constant
    int actualSize = 0;
    
    while (actualSize < sizeInBytes)
    {
        var numberLen = rnd.Next(2, 8) * 4;
        var textLen = rnd.Next(10, 100);

        var numberRaw = new byte[numberLen];
        var textRaw = new byte[textLen];
        rnd.NextBytes(numberRaw);
        rnd.NextBytes(textRaw);
        actualSize += numberLen + textLen;
        var text = System.Text.Encoding.ASCII.GetString(textRaw.Select(n => (byte)(97 + n % 25)).ToArray());
        
        StringBuilder numberBuilder = new StringBuilder(numberLen);
        for (int p = 0; p < numberRaw.Length; p += 4)
        {
            var n = BitConverter.ToUInt32(numberRaw, p);
            numberBuilder.Append(n);
        }

        yield return $"{numberBuilder}.{text}";
    }
}

[DataContract]
public class GenerateSettings
{
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
    public LinePartGenerateSettings NumberSize { get; set; }

    [DataMember]
    public LinePartGenerateSettings TextSize { get; set; }
}

[DataContract]
public class LinePartGenerateSettings
{
    [DataMember]
    public uint Min { get; set; }

    [DataMember]
    public uint Max { get; set; }
}