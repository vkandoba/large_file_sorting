using LargeFileSortingApp;
using LargeFileSortingApp.FileIO;
using LargeGenerateApp;
using NUnit.Framework;

namespace LargeFileSortingTests;

[TestFixture, Explicit]
public class ProcessFilePerfomanceTests
{
    private string file;
    private const long __CHECK_SUM = 374743629;

    [OneTimeSetUp]
    public void Init()
    {
        var dir = Directory.GetCurrentDirectory();
        file = FileHelpers.GenerateUniqueFileName(dir);
        var generateService = new GenerateService(new Random(42), null, null, null);
        var data = generateService.MakeRandomLines(512);
        File.WriteAllLines(file, data);
    }

    [Test]
    public void TestFileReadAllLines()
    {
        Utils.MeasureTime(() =>
        {
            var lines = File.ReadAllLines(file);
            long sum = lines.Select(LineItem.Parse).Select(x => (long)x.StringPart.Length).Sum();
            Assert.That(sum, Is.EqualTo(__CHECK_SUM));
        }, time => Console.Error.WriteLine($"{nameof(TestFileReadAllLines)}: {time} ms"));
    }

    [Test]
    public void TestFileReadLines()
    {
        Utils.MeasureTime(() =>
        {
            var lines = File.ReadLines(file);
            long sum = lines.Select(LineItem.Parse).Select(x => (long)x.StringPart.Length).Sum();
            Assert.That(sum, Is.EqualTo(__CHECK_SUM));
        }, time => Console.Error.WriteLine($"{nameof(TestFileReadLines)}: {time} ms"));
    }

    [Test]
    public void TestReadItems()
    {
        Utils.MeasureTime(() =>
        {
            var sum = FileHelpers.ReadLineItems(file).Select(x => (long)x.StringPart.Length).Sum();
            Assert.That(sum, Is.EqualTo(__CHECK_SUM));
        }, time => Console.Error.WriteLine($"{nameof(TestReadItems)}: {time} ms"));
    }
    
    [Test]
    public async Task TestReadItemsAsync()
    {
        await Utils.MeasureTime(async () =>
        {
            var items = FileHelpers.ReadLineItemsAsync(file);
            long sum = 0;
            await foreach (var item in items)
            {
                sum += item.StringPart.Length;
            }
            Assert.That(sum, Is.EqualTo(__CHECK_SUM));
        }, time => Console.Error.WriteLine($"{nameof(TestReadItemsAsync)}: {time} ms"));
    }

    [Test]
    public void TestChunkReader()
    {
        Utils.MeasureTime(() =>
        {
            long sum = 0;
            using var reader = new FileChunkLineReader(file, Constants.FileOpBufferSizeB, 64 * 1024 * 1024);
            var chunks = reader.ReadChunks();
            foreach (var chunk in chunks)
            {
                sum += chunk.Select(item => (long)item.StringPart.Length).Sum();
            }
            Assert.That(sum, Is.EqualTo(__CHECK_SUM));
        }, time => Console.Error.WriteLine($"{nameof(FileChunkLineReader)}: {time} ms"));
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        File.Delete(file);
    }
}