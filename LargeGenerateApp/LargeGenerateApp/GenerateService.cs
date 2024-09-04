using System.Runtime.ExceptionServices;
using System.Text;

namespace LargeGenerateApp;

public class GenerateService
{
    private readonly Random _rnd;
    
    private readonly NumberPartGenerateSettings _numberLimits;
    
    private readonly TextPartGenerateSettings _textLenLimits;
    
    private readonly LineDuplicatedSettings _duplicatedSettings;

    private const int MbScale = 1024 * 1024;

    private const string English = "abcdefghijklmnopqrstuvwxyz";

    private const string Serbian = "абвгдђежзијклљмнњопрстћуфхцчџ";

    private const string Symbols = $"{English}{Serbian}";
    
    public GenerateService(
        Random random,
        NumberPartGenerateSettings? numberLimits, 
        TextPartGenerateSettings? textLenLimits,
        LineDuplicatedSettings? duplicatedSettings)
    {
        _rnd = random;
        _duplicatedSettings = duplicatedSettings ?? new LineDuplicatedSettings{Rate = 0, Line = ""};
        _numberLimits = numberLimits ?? new NumberPartGenerateSettings{Min = long.MinValue, Max = long.MaxValue};
        _textLenLimits = textLenLimits ?? new TextPartGenerateSettings{Min = 0, Max = 1000};
    }
    
    public IEnumerable<string> MakeRandomLines(double totalSizeMb)
    {
        ulong sizeInBytes = (ulong)Math.Round(totalSizeMb * MbScale);
        ulong actualSizeInBytes = 0;
        ulong duplicatedLineSize = (ulong) Encoding.UTF8.GetByteCount(_duplicatedSettings.Line);
        
        while (actualSizeInBytes < sizeInBytes)
        {
            var isDuplicated = _rnd.NextDouble() < _duplicatedSettings.Rate;
            if (!isDuplicated)
            {
                var number = _rnd.NextInt64(_numberLimits.Min, _numberLimits.Max);
                var textLen = _rnd.Next(_textLenLimits.Min, _textLenLimits.Max);
                var line = GenNextLine(number, textLen);
                
                actualSizeInBytes += (ulong) Encoding.UTF8.GetByteCount(line) + 1; // 1 for '\n'
                yield return line;
            }
            else
            {
                actualSizeInBytes += duplicatedLineSize + 1;
                yield return _duplicatedSettings.Line;
            }
        }
    }

    private string GenNextLine(long number, int textLen)
    {
        var indexes = new byte[textLen];
        _rnd.NextBytes(indexes);
        var text = new string(indexes.Select(n => Symbols[(int)(n % Symbols.Length)]).ToArray());
        
        return $"{number}.{text}";
    }
}