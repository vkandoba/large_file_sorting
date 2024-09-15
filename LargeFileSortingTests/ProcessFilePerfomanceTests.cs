using LargeFileSortingApp;
using LargeFileSortingApp.FileIO;
using LargeGenerateApp;
using NUnit.Framework;

namespace LargeFileSortingTests;

[TestFixture, Explicit]
public class ProcessFilePerfomanceTests
{
    private string file;

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
    public void TestReadAllLines()
    {
        Utils.MeasureTime(() =>
        {
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                LineItem.Parse(line);
            }
        }, time => Console.Error.WriteLine($"ReadAllLines: {time} ms"));
    }

    [Test]
    public void TestReadItems()
    {
        Utils.MeasureTime(() =>
        {
            var items = FileHelpers.ReadLineItems(file);
            foreach (var item in items)
            {
            }
        }, time => Console.Error.WriteLine($"{nameof(FileHelpers.ReadLineItems)}: {time} ms"));
    }

    [Test]
    public void TestChunkReader()
    {
        Utils.MeasureTime(() =>
        {
            using var reader = new FileChunkLineReader(file, Constants.FileOpBufferSizeB, 128 * 1024 * 1024);
            var chunks = reader.ReadChunks();
            foreach (var chunk in chunks)
            {
                foreach (var item in chunk)
                {
                }
            }
        }, time => Console.Error.WriteLine($"{nameof(FileChunkLineReader)}: {time} ms"));
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        File.Delete(file);
    }
}