// See https://aka.ms/new-console-template for more information

using System.Text;

Console.WriteLine("Start generate");
var rnd = new Random();
var fileName = "/users/vkandoba/data/test";

var lines = GenerateLines();
File.Delete(fileName);
File.AppendAllLines(fileName, lines);
var fileAttr = new FileInfo(fileName);
Console.WriteLine($"{fileAttr.Length / 1024.0:n2} KB");


IEnumerable<string> GenerateLines()
{
    var linesCount = 10000000;
    for (int i = 0; i < linesCount; i++)
    {
        var numberLen = rnd.Next(2, 8) * 4;
        var textLen = rnd.Next(10, 100);

        // Console.WriteLine($"{numberLen}. {textLen}");
        var numberRaw = new byte[numberLen];
        var textRaw = new byte[textLen];
        rnd.NextBytes(numberRaw);
        rnd.NextBytes(textRaw); 
        var text = System.Text.Encoding.ASCII.GetString(textRaw.Select(n => (byte)(97 + n % 25)).ToArray());
        
        StringBuilder numberBuilder = new StringBuilder(numberLen);
        for (int p = 0; p < numberRaw.Length; p += 4)
        {
            var n = BitConverter.ToUInt32(numberRaw, p);
            numberBuilder.Append(n);
        }

        // Console.WriteLine($"{numberBuilder}.{text}");
        yield return $"{numberBuilder}.{text}";
    }
}

