using LargeFileSortingApp;
using LargeFileSortingApp.SortingService;
using NUnit.Framework;

namespace LargeFileSortingTests;

[TestFixture]
public class ParseLineItemTests
{
    [Test]
    public void ParseLineItemTest()
    {
        var item = LineItem.Parse("76635.Apple");

        Assert.That(item.Line, Is.EqualTo("76635.Apple"));
        Assert.That(item.Number.ToString(), Is.EqualTo("76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo("Apple"));
    }
    
    [Test]
    public void ParseLineItemWhenStringPartIsEmptyTest()
    {
        var item = LineItem.Parse("76635.");

        Assert.That(item.Line, Is.EqualTo("76635."));
        Assert.That(item.Number.ToString(), Is.EqualTo("76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void ParseLineItemWhenStringPartContainsDotTest()
    {
        var item = LineItem.Parse("76635.Apple.Apple2.");

        Assert.That(item.Line, Is.EqualTo("76635.Apple.Apple2."));
        Assert.That(item.Number.ToString(), Is.EqualTo("76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo("Apple.Apple2."));
    }
    
    [Test]
    public void ParseLineItemWhenNumberIsZero()
    {
        var item = LineItem.Parse("0.");

        Assert.That(item.Line, Is.EqualTo("0."));
        Assert.That(item.Number.ToString(), Is.EqualTo("0"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo(""));
    }    

    [Test]
    public void ParseLineItemWhenNumberIsNegative()
    {
        var item = LineItem.Parse("-76635.");

        Assert.That(item.Line, Is.EqualTo("-76635."));
        Assert.That(item.Number.ToString(), Is.EqualTo("-76635"));
        Assert.That(item.StringPart.ToString(), Is.EqualTo(""));
    }    
    
    //TODO: test for oversize number

    [Test]
    public void ParseLineItemWithoutDots()
    {
        Assert.Throws<ArgumentException>(() => LineItem.Parse("76635"));
    }
    
    [Test]
    public void ParseLineWhenNumberPartIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => LineItem.Parse(".Apple"));
    }

    [Test]
    public void ParseLineWhenIsNull()
    {
        Assert.Throws<ArgumentException>(() => LineItem.Parse(null));
    }
}