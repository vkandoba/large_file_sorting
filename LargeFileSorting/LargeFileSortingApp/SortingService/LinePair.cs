using System.Text;

namespace LargeFileSortingApp.SortingService;

public class LinePair : IComparable
{
    public readonly string Line;

    private int _stringPartStartIndex;

    private int _stringPartLength;

    private ReadOnlySpan<char> NumberPart => Line.AsSpan(0, _stringPartStartIndex - 1);

    private ReadOnlySpan<char> StringPart => Line.AsSpan(_stringPartStartIndex, _stringPartLength);
    
    private LinePair(string line, int startStringPart)
    {
        Line = line;
        _stringPartStartIndex = startStringPart;
        _stringPartLength = line.Length - startStringPart;
    }
    
    public static LinePair Parse(string line)
    {
        var dotIndex = line.IndexOf('.');
        if (dotIndex == -1)
            throw new ArgumentException($"Failed to parse line: {line}");
        return new LinePair(line, dotIndex + 1);
    }
    
    public int CompareTo(object? obj)
    {
        var other = obj as LinePair;
        if (other == null)
            return 1;

        var stringCompareResult = this.StringPart.CompareTo(other.StringPart, StringComparison.Ordinal);
        if (stringCompareResult != 0)
            return stringCompareResult;
        
        if (_stringPartStartIndex == other._stringPartStartIndex)
            return NumberPart.CompareTo(other.NumberPart, StringComparison.Ordinal);

        return _stringPartStartIndex.CompareTo(_stringPartStartIndex);
    }
}