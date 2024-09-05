namespace LargeFileSortingApp;

public class LineItem : IComparable
{
    public readonly string Line;
    
    public readonly long Number;
    
    public ReadOnlySpan<char> StringPart => Line.AsSpan(_stringPartStartIndex, _stringPartLength);
    
    private int _stringPartStartIndex;

    private int _stringPartLength;
   
    private LineItem(string line, long number, int startStringPart)
    {
        Line = line;
        Number = number;
        _stringPartStartIndex = startStringPart;
        _stringPartLength = line.Length - startStringPart;
    }
    
    public static LineItem Parse(string line)
    {
        if (string.IsNullOrEmpty(line))
            throw new ArgumentException($"Failed to parse line: {line}. It's null or empty");
        
        var dotIndex = line.IndexOf('.');
        if (dotIndex == -1)
            throw new ArgumentException($"Failed to parse line: {line}. There no the dot character");
        
        if (dotIndex == 0)
            throw new ArgumentException($"Failed to parse line: {line}. The number part is empty");

        var numberPart = line.AsSpan(0, dotIndex);
        if (!long.TryParse(numberPart, out var  number))
            throw new ArgumentException($"Failed to parse line: {line}. The number part is incorrect");
        
        return new LineItem(line, number, dotIndex + 1);
    }
    
    public int CompareTo(object? obj)
    {
        var other = obj as LineItem;
        if (other == null)
            return 1;

        var stringCompareResult = this.StringPart.CompareTo(other.StringPart, StringComparison.Ordinal);
        if (stringCompareResult != 0)
            return stringCompareResult;

        return Number.CompareTo(other.Number);
    }
}