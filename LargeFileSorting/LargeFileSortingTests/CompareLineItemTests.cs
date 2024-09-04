using LargeFileSortingApp;
using NUnit.Framework;

namespace LargeFileSortingTests;

[TestFixture]
public class CompareLineItemTests
{
    [Test]
    public void CompareLinesByStringPart()
    {
        var item1 = LineItem.Parse("11111.BApple");
        var item2 = LineItem.Parse("76635.AApple");

        Assert.That(item1.CompareTo(item2), Is.EqualTo(1));
    }

    [Test]
    public void CompareLinesByNumberPart()
    {
        var item1 = LineItem.Parse("12.");
        var item2 = LineItem.Parse("11.");

        Assert.That(item1.CompareTo(item2), Is.EqualTo(1));
    }

    [Test]
    public void CompareLinesByNumberPartLength()
    {
        var item1 = LineItem.Parse("100.");
        var item2 = LineItem.Parse("23.");

        Assert.That(item1.CompareTo(item2), Is.EqualTo(1));
    }

    [Test]
    public void CompareLinesWhenEqual()
    {
        var item1 = LineItem.Parse("76635.Apple");
        var item2 = LineItem.Parse("76635.Apple");

        Assert.That(item1.CompareTo(item2), Is.EqualTo(0));
    }

    [Test]
    public void CompareLinesWhenOtherIsNull()
    {
        var item1 = LineItem.Parse("76635.Apple");

        Assert.That(item1.CompareTo(null), Is.EqualTo(1));
    }
}