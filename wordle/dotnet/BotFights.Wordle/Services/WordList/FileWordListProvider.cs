using System.Text;

namespace BotFights.Wordle.Services.WordList;

public class FileWordListProvider : IWordListProvider
{
    private const string Path = @"..\..\..\..\..\python\wordlist.txt";
    private List<string> _lines = null;

    public Task<List<string>> GetWordList()
    {
        if (_lines != null)
        {
            return Task.FromResult(_lines);
        }

        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);

        string line;
        var lines = new List<string>();

        while ((line = sr.ReadLine()) != null)
        {
            lines.Add(line.ToLowerInvariant());
        }

        _lines = lines.ToList();
        return Task.FromResult(_lines);
    }
}