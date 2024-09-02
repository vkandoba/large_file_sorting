namespace LargeGenerateApp;

public class GenerateService
{
    private const int mbScale = 1024 * 1024;

    public IEnumerable<string> MakeLines(GenerateSettings settings)
    {
        var rnd = new Random(settings.RandomSeed);

        ulong sizeInBytes = (ulong)(settings.File.MinSizeMb * mbScale);
        ulong actualSizeInBytes = 0;
    
        while (actualSizeInBytes < sizeInBytes)
        {
            var numberRaw = (ulong) rnd.NextInt64(settings.Line.Number.Min, settings.Line.Number.Max);
            var numberStr = numberRaw.ToString();
        
            var textLen = rnd.Next((int)settings.Line.TextSize.Min, (int)settings.Line.TextSize.Max);
            var textRaw = new byte[textLen];
            rnd.NextBytes(textRaw);
            // TODO: get different utf-8 symbols from set
            var text = System.Text.Encoding.ASCII.GetString(textRaw.Select(n => (byte)(97 + n % 25)).ToArray());
        
            actualSizeInBytes += (ulong) (numberStr.Length + textLen);
            yield return $"{numberStr}.{text}";
        }
    }
}