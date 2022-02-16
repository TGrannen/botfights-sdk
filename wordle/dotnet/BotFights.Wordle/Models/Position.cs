namespace BotFights.Wordle.Models;

public class Position
{
    public PositionResult Result { get; set; }
    public char Char { get; set; }
    public int Index { get; set; }
}