using ShogiBoard.Models;
using System.Collections.Generic;

namespace ShogiBoard.Services;

public interface IGameService
{
    void InitBoardData(string[,] boardData, int boardLength);
    void UpdateBoardData(string sfenBoardText, string[,] boardData, int boardLength);
    string CreateSfenFromMoves(List<Move> moves);
    bool IsValidMove(Move m);
}

public class GameService : IGameService
{
    private const string startPos = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL";

    public void InitBoardData(string[,] boardData, int boardLength)
    {
        UpdateBoardData(startPos, boardData, boardLength);
    }

    public void UpdateBoardData(string sfenBoardText, string[,] boardData, int boardLength)
    {
        for (int i = 0, file = 0, rank = 0 ; i < sfenBoardText.Length; ++i )
        {
            char ch = sfenBoardText[i];
            if (ch == '/')
            {
                file = 0;
                ++rank;
                if (boardLength < rank + 1)
                {
                    throw new System.Exception($"Got too many '/' for the board length");
                }
                continue;
            }
            else if (ch == '+')
            {
                string s = sfenBoardText.Substring(i, 2);
                if (!IsValidPieceChar(s[1]))
                {
                    throw new System.Exception($"Unexpected character at position ({i + 1}) from {sfenBoardText}. Got {s[1]}");
                };
                boardData[rank, file] = s;
                i += 1;
                ++file;
            }
            else if (int.TryParse(ch.ToString(), out var n))
            {
                if (n < 0 || (boardLength - file) < n) throw new System.Exception($"Unexpected character at position ({i}) from {sfenBoardText}. Got {n}");

                for (int _n = 0; _n < n; _n++)
                {
                    boardData[rank, file + _n] = string.Empty;
                }

                file += n;
            }
            else if (IsValidPieceChar(ch))
            {
                boardData[rank, file] = ch.ToString();
                ++file;
            }
            else
            {
                throw new System.Exception($"Unexpected character at position ({i}) from {sfenBoardText}. Got ({ch})");
            }

            if (boardLength < file)
            {
                throw new System.Exception($"Too long width({file + 1}) for the board({boardLength}, {boardLength}). (index: {i}, ch: {ch})");
            }
        }
    }

    public bool IsValidMove(Move m)
    {
        // TODO
        return true;
    }

    public string CreateSfenFromMoves(List<Move> moves)
    {
        // TODO
        return "";
    }

    private static bool IsValidPieceChar(char ch)
    {
        return ch switch
        {
            'p' or 'l' or 'n' or 's' or 'g' or 'k' or 'b' or 'r'  => true,
            'P' or 'L' or 'N' or 'S' or 'G' or 'K' or 'B' or 'R'  => true,
            _ => false
        };
    }
}