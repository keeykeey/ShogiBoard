using System;
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

public partial struct Sfen
{
    private const string sfenStart = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1";
    public Sfen()
    {
        var result = SfenParser.Split(sfenStart) ?? throw new ArgumentException("Invalid SFEN text");
        var (b, bw, sp, bsp, wsp, mc) = result;
        Board = b;
        BlackWhite = bw;
        SidePiece = sp;
        BlackSidePiece = bsp;
        WhiteSidePiece = wsp;
        MoveCount = mc;
    }

    public Sfen(string sfenStyleText)
    {
        var result = SfenParser.Split(sfenStyleText) ?? throw new ArgumentException("Invalid SFEN text");
        var (b, bw, sp, bsp, wsp, mc) = result;
        Board = b;
        BlackWhite = bw;
        SidePiece = sp;
        BlackSidePiece = bsp;
        WhiteSidePiece = wsp;
        MoveCount = mc;
    }

    public void Update(string sfenStyleText)
    {
        var result = SfenParser.Split(sfenStyleText) ?? throw new ArgumentException("Invalid SFEN text");
        var (b, bw, sp, bsp, wsp, mc) = result;
        Board = b;
        BlackWhite = bw;
        SidePiece = sp;
        BlackSidePiece = bsp;
        WhiteSidePiece = wsp;
        MoveCount = mc;
    }

    public string Board { get; set; } = string.Empty;
    public string BlackWhite { get; set; } = string.Empty;
    public string SidePiece { get; set; } = string.Empty;
    public string BlackSidePiece { get; set; } = string.Empty;
    public string WhiteSidePiece { get; set; } = string.Empty;
    public int MoveCount { get; set; } = default;
}

public partial class SfenParser
{
    [GeneratedRegex(@"^(?<board>(([\dplnsgkrbPLNSGKRB(+p)(+l)(+n)(+s)(+g)(+k)(+r)(+b)(+P)(+L)(+N)(+S)(+G)(+K)(+R)(+B)]{1,9}/){8}([\dplnsgkrbPLNSGKRB(+p)(+l)(+n)(+s)(+g)(+k)(+r)(+b)(+P)(+L)(+N)(+S)(+G)(+K)(+R)(+B)]{1,9})))\s(?<blackWhite>[bw])\s(?<sidePiece>([dplnsgkrbPLNSGKRB]\d{0,2}|-)*)\s(?<moveCount>\d+)$")]
    public static partial Regex SfenRegex();

    [GeneratedRegex(@"([PLNSGKRB]\d{0,2}|-)")]
    public static partial Regex BlackSidePieceRegex();

    [GeneratedRegex(@"([plnsgkrb]\d{0,2}|-)")]
    public static partial Regex WhiteSidePieceRegex();

    public static (
        string board,
        string blackWhite,
        string sidePiece,
        string blackSidePiece,
        string whiteSidePiece,
        int moveCount
    )? Split(string sfen)
    {
        Match match = SfenRegex().Match(sfen);
        if (match.Success)
        {
            string board = match.Groups["board"].Value;
            string bw = match.Groups["blackWhite"].Value;
            string sp = match.Groups["sidePiece"].Value;
            int mc = int.Parse(match.Groups["moveCount"].Value); // we know moveCount has number string.

            string blackSidePiece = WhiteSidePieceRegex().Replace(sp, "");
            string whiteSidePiece = BlackSidePieceRegex().Replace(sp, "");
            return (board, bw, sp, blackSidePiece, whiteSidePiece, mc);
        }
        return null;
    }

    private static string? FindByKey(string sfen, string key)
    {
        Match match = SfenRegex().Match(sfen);
        if (match.Success)
        {
            string sidePiece = match.Groups[key].Value;
            return sidePiece;
        }
        return null;
    }

    public static string? GetBoard(string sfen)
    {
        string? s = FindByKey(sfen, "board");
        return s;
    }

    public static string? GetBlackWhite(string sfen)
    {
        string? s = FindByKey(sfen, "blackWhite");
        return s;
    }

    public static string? GetSidePiece(string sfen)
    {
        string? s = FindByKey(sfen, "sidePiece");
        return s;
    }

    public static string? GetMoveCount(string sfen)
    {
        string? s = FindByKey(sfen, "moveCount");
        return s;
    }
}
