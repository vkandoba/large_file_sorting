namespace LargeFileSortingApp.SortingService;

public class LinePair : IComparable
{
    public string Number { get; set; }
    public string String { get; set; }
    
    public static LinePair Parse(string line)
    {
        var dotIndex = line.IndexOf('.');
        if (dotIndex == -1)
            throw new ArgumentException($"Failed to parse line: {line}");
        var numberPart = line.Substring(0, dotIndex + 1);
        var stringPart = line.Substring(dotIndex + 1, line.Length - dotIndex - 1);
        return new LinePair { Number = numberPart, String = stringPart };
    }

    public string MakeString() => $"{Number}.{String}";
    
    public int CompareTo(object? obj)
    {
        var other = obj as LinePair; // TODO: check when obj is null

        var stringCompareResult = String.Compare(String, other?.String, StringComparison.Ordinal);
        if (stringCompareResult != 0)
            return stringCompareResult;
        
        //TODO: test and implement by correct way 
        return String.Compare(Number, other?.Number, StringComparison.Ordinal);
    }
}

public interface ISortingService
{
    IEnumerable<LinePair> Sort(IEnumerable<LinePair> lines);
}