using System.Text;

namespace BotFights.Wordle.Services.WordList;

public class FileWordListProvider : IWordListProvider
{
    private const string BasePath = @".\Wordlist\";
    private readonly Dictionary<string, List<string>> _linesDictionary = new();

    public Task<List<string>> GetWordList(string file)
    {
        if (_linesDictionary.ContainsKey(file))
        {
            return Task.FromResult(_linesDictionary[file]);
        }

        using var fs = new FileStream(Path.Combine(BasePath, file), FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);

        string line;
        var lines = new List<string>();

        while ((line = sr.ReadLine()) != null)
        {
            lines.Add(line.ToLowerInvariant());
        }

        _linesDictionary.Add(file, lines);
        return Task.FromResult(lines);
    }
}