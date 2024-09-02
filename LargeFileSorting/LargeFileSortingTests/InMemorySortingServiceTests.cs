using System.Xml.Serialization;
using LargeFileSortingApp;
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

    [Test]
    public void TestSortNumberLines()
    {
        var lines = new []
        {
            LineItem.Parse("-32."),
            LineItem.Parse("415."),
            LineItem.Parse("30433."),
            LineItem.Parse("1."),
            LineItem.Parse("2."),
            LineItem.Parse("30432896523."),
            LineItem.Parse("0."),
        };

        var result = _service.Sort(lines).Select(x => x.Line).ToArray();
        
        var expected = new[]
        {
            "-32.",
            "0.",
            "1.",
            "2.",
            "415.",
            "30433.",
            "30432896523."
        };

        Assert.That(result, Is.EqualTo(expected).AsCollection);
    }
}