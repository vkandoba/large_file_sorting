using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace LargeGenerateApp;

[DataContract]
public class GenerateSettings
{
    [DataMember]
    public int RandomSeed { get; set; }
    
    [DataMember]
    public double MinSizeMb { get; set; } // reason of double - generating a small file, like 0.2 MB
 
    [DataMember]
    public LineDuplicatedSettings Duplicated { get; set; }

    [DataMember]
    public LineGeneratedSettings Generated { get; set; }

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

public class LineDuplicatedSettings
{
    [DataMember]
    public double Rate { get; set; } // should be from 0 to 1
    
    [DataMember]
    public string Line { get; set; }
}

[DataContract]
public class LineGeneratedSettings
{
    [DataMember]
    public NumberPartGenerateSettings? NumberPart { get; set; }

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