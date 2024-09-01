using LargeFileSortingApp.SortingService;
using NUnit.Framework;

namespace LargeFileSortingTests;

[TestFixture]
public class LineItemParseTests
{
    [Test]
    public void ParseLineItemTest()
    {
        var item = LineItem.Parse("76635.Apple");

        Assert.That(item.Line, Is.EqualTo("76635.Apple"));
        Assert.That(item.NumberPart.ToString(), Is.EqualTo("76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo("Apple"));
    }
    
    [Test]
    public void ParseLineItemWhenStringPartIsEmptyTest()
    {
        var item = LineItem.Parse("76635.");

        Assert.That(item.Line, Is.EqualTo("76635."));
        Assert.That(item.NumberPart.ToString(), Is.EqualTo("76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void ParseLineItemWhenStringPartContainsDotTest()
    {
        var item = LineItem.Parse("76635.Apple.Apple2.");

        Assert.That(item.Line, Is.EqualTo("76635.Apple.Apple2."));
        Assert.That(item.NumberPart.ToString(), Is.EqualTo("76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo("Apple.Apple2."));
    }

    
    [Test]
    public void ParseLineItemWithoutDots()
    {
        Assert.Throws<ArgumentException>(() => LineItem.Parse("76635"));
    }
   
    [Test]
    public void ParseLineWhenStartsFromZero()
    {
        Assert.Throws<ArgumentException>(() => LineItem.Parse("0013.Apple"));
    }

}