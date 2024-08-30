namespace LargeFileSortingApp.SortingService;

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

    public static LinePair Parse(string line)
    {
        var tokens = line.Split('.'); // TODO: many points in string
        return new LinePair { Number = tokens[0], String = tokens[1] };
    }

    public string MakeString() => $"{Number}.{String}";
}

public interface ISortingService
{
    IEnumerable<LinePair> Sort(IEnumerable<LinePair> lines);
}