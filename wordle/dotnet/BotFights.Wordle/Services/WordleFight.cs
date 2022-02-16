namespace BotFights.Wordle.Services;

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

public class Game
{
    public int Number { get; set; }
    public List<Try> Tries { get; set; }
    public bool Solved { get; set; } = false;
}

public class Try
{
    public string TryString { get; set; }
    public List<Position> Positions { get; set; }
}

public class Position
{
    public PositionResult Result { get; set; }
    public char Char { get; set; }
    public int Index { get; set; }
}

public enum PositionResult
{
    Correct,
    Close,
    Miss
}

public class Guess
{
    public int GameNumber { get; set; }
    public string GuessString { get; set; }
}