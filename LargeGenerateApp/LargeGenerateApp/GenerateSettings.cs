using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace LargeGenerateApp;

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
    public string DefaultName { get; set; }

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