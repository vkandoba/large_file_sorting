using System.Text;

namespace LargeFileSortingApp.SortingService;

public class LineItem : IComparable
{
    public readonly string Line;
    
    public ReadOnlySpan<char> NumberPart => Line.AsSpan(0, _stringPartStartIndex - 1);

    public ReadOnlySpan<char> StringPart => Line.AsSpan(_stringPartStartIndex, _stringPartLength);
    
    private int _stringPartStartIndex;

    private int _stringPartLength;
   
    private LineItem(string line, int startStringPart)
    {
        Line = line;
        _stringPartStartIndex = startStringPart;
        _stringPartLength = line.Length - startStringPart;
    }
    
    public static LineItem Parse(string line)
    {
        if (string.IsNullOrEmpty(line))
            throw new ArgumentException($"Failed to parse line: {line}. It's null or empty");
        
        // TODO: Check that number is positive 
        
        var dotIndex = line.IndexOf('.');
        if (dotIndex == -1)
            throw new ArgumentException($"Failed to parse line: {line}. There no the dot character");
        
        if (line[0] == '0' && dotIndex != 1 ) // Numbers with lead zeros break the number comparing
            throw new ArgumentException($"Failed to parse line: {line}. The number part starts from lead zero");

        if (dotIndex == 0)
            throw new ArgumentException($"Failed to parse line: {line}. The number part is empty");
        
        return new LineItem(line, dotIndex + 1);
    }
    
    public int CompareTo(object? obj)
    {
        var other = obj as LineItem;
        if (other == null)
            return 1;

        var stringCompareResult = this.StringPart.CompareTo(other.StringPart, StringComparison.Ordinal);
        if (stringCompareResult != 0)
            return stringCompareResult;
        
        if (_stringPartStartIndex == other._stringPartStartIndex) // In fact, comparing number length
            return NumberPart.CompareTo(other.NumberPart, StringComparison.Ordinal);

        // TODO: maybe parse and place number in memory 
        return _stringPartStartIndex.CompareTo(other._stringPartStartIndex);
    }
}