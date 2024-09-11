using System.Text;

namespace LargeFileSortingApp.FileIO;

public class FileChunkLineReader : IFileChunkLineReader
{
    private readonly FileStream _stream;
    private readonly StreamReader _reader;
    
    private readonly int _bufferSizeB;
    private readonly int _chunkSizeB;

    public FileChunkLineReader(string filename, int bufferSizeB, int chunkSizeB)
    {
        _stream = File.OpenRead(filename);
        _reader = new StreamReader(_stream, Encoding.UTF8, bufferSize: bufferSizeB);
        _bufferSizeB = bufferSizeB;
        _chunkSizeB = chunkSizeB;
    }

    public async IAsyncEnumerable<LineItem[]> ReadChunks()
    {
        IList<LineItem> current = new List<LineItem>(_chunkSizeB / (2 * 1024));
        var lastPos = _reader.BaseStream.Position;
        string? line;
        while ((line = await _reader.ReadLineAsync()) != null)
        {
            var item = LineItem.Parse(line);
            current.Add(item);
            if (_reader.BaseStream.Position - _bufferSizeB > lastPos + _chunkSizeB) 
            {
                yield return current.ToArray();
                current.Clear();
                lastPos = _reader.BaseStream.Position - _bufferSizeB;
            }
        }
        
        if (current.Any())
            yield return current.ToArray();
    }

    public void Dispose()
    {
        _reader.Dispose();
        _stream.Dispose();
    }
}