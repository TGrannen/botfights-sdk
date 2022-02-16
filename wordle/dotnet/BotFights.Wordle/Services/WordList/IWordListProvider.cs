namespace BotFights.Wordle.Services.WordList;

public interface IWordListProvider
{
    public Task<List<string>> GetWordList();
}