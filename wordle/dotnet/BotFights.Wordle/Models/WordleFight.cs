namespace BotFights.Wordle.Models;

public class WordleFight
{
    public string Id { get; set; }
    public int Number { get; set; }
    public int GuessCount { get; set; }
    public int RoundNumber { get; set; }
    public string Wordlist { get; set; }
    public string Status { get; set; }
    public List<Game> Games { get; set; }
}