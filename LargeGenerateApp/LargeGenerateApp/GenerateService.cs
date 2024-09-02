using System.Text;

namespace LargeGenerateApp;

public class GenerateService
{
    private readonly Random _rnd;

    private readonly NumberPartGenerateSettings _numberLimits;
    
    private readonly TextPartGenerateSettings _textLenLimits;
    
    private const int MbScale = 1024 * 1024;

    private const string _english = "abcdefghijklmnopqrstuvwxyz";

    private const string _serbian = "абвгдђежзијклљмнњопрстћуфхцчџ";
    
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

        var symbols = $"{_english}{_serbian}";
        while (actualSizeInBytes < sizeInBytes)
        {
            var number = (ulong) _rnd.NextInt64(_numberLimits.Min, _numberLimits.Max);
        
            var textLen = _rnd.Next(_textLenLimits.Min, _textLenLimits.Max);
            var indexes = new byte[textLen];
            _rnd.NextBytes(indexes);

            var text = new string(indexes.Select(n => symbols[(int)(n % symbols.Length)]).ToArray());
            var textBytes = Encoding.UTF8.GetByteCount(text);
            
            actualSizeInBytes += (ulong) (number.ToString().Length + textBytes);
            yield return $"{number}.{text}";
        }
    }
}