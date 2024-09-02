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
    public double MinSizeMb { get; set; } // May be small, like 0.2 MB
}

[DataContract]
public class LineGenerateSettings
{
    [DataMember]
    public NumberPartGenerateSettings? Number { get; set; }

    [DataMember]
    public TextPartGenerateSettings? TextPartSize { get; set; }
}

[DataContract]
public class NumberPartGenerateSettings
{
    [DataMember]
    public long Min { get; set; }

    [DataMember]
    public long Max { get; set; }
}

[DataContract]
public class TextPartGenerateSettings
{
    [DataMember]
    public ushort Min { get; set; }

    [DataMember]
    public ushort Max { get; set; }
}