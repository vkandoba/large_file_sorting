namespace LargeFileSorting;

public class LinePair : IComparable
{
    public string Number { get; set; }
    public string String { get; set; }
    
    public int CompareTo(object? obj)
    {
        var other = obj as LinePair; // TODO: check when obj is null

        var stringCompareResult = String.Compare(String, other?.String, StringComparison.Ordinal);
        if (stringCompareResult != 0)
            return stringCompareResult;
        
        //TODO: test and implement by correct way 
        return String.Compare(Number, other?.Number, StringComparison.Ordinal);
    }

    public string MakeString() => $"{Number}.{String}";
}

public interface IFileSortingService
{
    IEnumerable<string> Do(IEnumerable<string> lines);
}

public class FileSortingService : IFileSortingService
{
    public IEnumerable<string> Do(IEnumerable<string> lines)
    {
        var linePairs = ParseLines(lines).ToArray();
        Array.Sort(linePairs);
        return linePairs.Select(x => x.MakeString());
    }

    private IEnumerable<LinePair> ParseLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var tokens = line.Split('.');
            yield return new LinePair { Number = tokens[0], String = tokens[1] };
        }
    }
}