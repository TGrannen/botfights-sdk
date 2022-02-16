namespace BotFights.Wordle.Models;

public class Game
{
    public int Number { get; set; }
    public List<Try> Tries { get; set; }
    public bool Solved { get; set; } = false;
}