namespace LargeGenerateApp;

public class GenerateService
{
    private readonly Random _rnd;

    private readonly NumberPartGenerateSettings _numberLimits;
    
    private readonly TextPartGenerateSettings _textLenLimits;
    
    private const int MbScale = 1024 * 1024;

    public GenerateService(
        Random random, 
        NumberPartGenerateSettings? numberLimits, 
        TextPartGenerateSettings? textLenLimits)
    {
        _rnd = random;
        _numberLimits = numberLimits ?? new NumberPartGenerateSettings{Min = long.MinValue, Max = long.MaxValue};
        _textLenLimits = textLenLimits ?? new TextPartGenerateSettings{Min = 0, Max = 1000};
    }
    
    public IEnumerable<string> MakeRandomLines(double totalSizeMb)
    {
        ulong sizeInBytes = (ulong)Math.Round(totalSizeMb * MbScale);
        ulong actualSizeInBytes = 0;
    
        while (actualSizeInBytes < sizeInBytes)
        {
            
            var numberRaw = (ulong) _rnd.NextInt64(_numberLimits.Min, _numberLimits.Max);
            var numberStr = numberRaw.ToString();
        
            var textLen = _rnd.Next(_textLenLimits.Min, _textLenLimits.Max);
            var textRaw = new byte[textLen];
            _rnd.NextBytes(textRaw);
            // TODO: get different utf-8 symbols from set
            var text = System.Text.Encoding.ASCII.GetString(textRaw.Select(n => (byte)(97 + n % 25)).ToArray());
        
            actualSizeInBytes += (ulong) (numberStr.Length + textLen);
            yield return $"{numberStr}.{text}";
        }
    }
}