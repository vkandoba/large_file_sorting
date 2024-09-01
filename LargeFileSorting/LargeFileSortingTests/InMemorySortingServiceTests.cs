using System.Xml.Serialization;
using LargeFileSortingApp.SortingService;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LargeFileSortingTests;

[TestFixture]
public class InMemorySortingServiceTests
{
    private InMemorySortingService _service;

    [SetUp]
    public void InitService()
    {
        _service = new InMemorySortingService();
    }

    [Test]
    public void TestSortLines()
    {
        var lines = new []
        {
            LineItem.Parse("415. Apple"),
            LineItem.Parse("30432. Something something something"),
            LineItem.Parse("1. Apple"),
            LineItem.Parse("32. Cherry is the best"),
            LineItem.Parse("2. Banana is yellow")
        };

        var result = _service.Sort(lines).Select(x => x.Line).ToArray();
        
        var expected = new[]
        {
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        };

        Assert.That(result, Is.EqualTo(expected).AsCollection);
    }
    
}