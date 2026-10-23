using System.Text.RegularExpressions;

namespace ShogiBoard.Models;

public static class UsiMessage
{
    public static readonly string USI = "usi";
    public static readonly string USI_OK = "usiok";
    public static readonly string IS_READY = "isready";
    public static readonly string READY_OK = "readyok";
    public static readonly string NEW_GAME = "usinewgame";
    public static readonly string SFEN_BASE_TEXT = "position startpos moves ";
    public static readonly string GO = "go";
    public static readonly string BEST_MOVE = "bestmove";
    public static readonly string RESIGN = "resign";
    public static readonly string STOP = "stop";
    public static readonly string GAME_OVER = "gameover";
    public static readonly string WIN = "win";
    public static readonly string LOSE = "lose";
    public static readonly string DRAW = "draw";
    public static readonly string QUIT = "quit";

}

public partial class UsiParser
{
    [GeneratedRegex(@"^bestmove\s+(?<best>[^\s]+)(?:\s+ponder\s+(?<ponder>[^\s]+))?")]
    private static partial Regex BestmoveRegex();

    public static string ParseBestmove(string input)
    {
        if (input.StartsWith($"{UsiMessage.BEST_MOVE} {UsiMessage.RESIGN}"))
        {
            return UsiMessage.RESIGN;
        }

        Match match = BestmoveRegex().Match(input);

        if (match.Success)
        {
            string bestMove = match.Groups["best"].Value;
            return bestMove;
        }

        return "";
    }
}
