using ShogiBoard.Models;

namespace ShogiBoard.Services;

public interface IGameService
{
    void Init9DimBoardData(GameState gameState);
    void UpdateBoardData(GameState gameState, string sfenBoardText);
    void BoardDataNext(IPosition from, Tile to, bool promote, GameState state);
    void NextTurn(GameState state);
    bool IsValidMove(IPosition from, IPosition to);
    bool CanPromote(Tile fromTileRank, Tile destTileRank, GameState state);
}

public class GameService : IGameService
{
    private const string startPos9Dim = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL";
    private int BlackPromotionLine { get; set; } = 3;
    private int WhitePromotionLine { get; set; } = 7;

    public void Init9DimBoardData(GameState gameState)
    {
        UpdateBoardData(gameState, startPos9Dim);
    }

    /* It sets new board data into a 2-dimentional board data matrix */
    public void UpdateBoardData(GameState gameState, string sfenBoardText)
    {
        for (int i = 0, file = 0, rank = 0 ; i < sfenBoardText.Length; ++i )
        {
            char ch = sfenBoardText[i];
            if (ch == '/')
            {
                file = 0;
                ++rank;
                if (gameState.BoardLength < rank + 1)
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
                gameState.BoardData[rank, file] = s;
                i += 1;
                ++file;
            }
            else if (int.TryParse(ch.ToString(), out var n))
            {
                if (n < 0 || (gameState.BoardLength - file) < n) throw new System.Exception($"Unexpected character at position ({i}) from {sfenBoardText}. Got {n}");

                for (int _n = 0; _n < n; _n++)
                {
                    gameState.BoardData[rank, file + _n] = string.Empty;
                }

                file += n;
            }
            else if (IsValidPieceChar(ch))
            {
                gameState.BoardData[rank, file] = ch.ToString();
                ++file;
            }
            else
            {
                throw new System.Exception($"Unexpected character at position ({i}) from {sfenBoardText}. Got ({ch})");
            }

            if (gameState.BoardLength < file)
            {
                throw new System.Exception($"Too long width({file + 1}) for the board({gameState.BoardLength}, {gameState.BoardLength}). (index: {i}, ch: {ch})");
            }
        }
    }

    public bool IsValidMove(IPosition from, IPosition to)
    {
        // TODO
        return true;
    }

    public void BoardDataNext(IPosition from, Tile to, bool promote, GameState state)
    {
        int toRankIndex = to.Ranks - 1;
        int toFileIndex = state.BoardLength - to.Files;

        if (from is Tile tile)
        {
            int fromRankIndex = tile.Ranks - 1;
            int fromFileIndex = state.BoardLength - tile.Files;
            string doPromoteStr = promote ? "+" : "";
            string movingPiece = $"{doPromoteStr}{state.BoardData[fromRankIndex, fromFileIndex]}";
            string pieceToBeTaken = state.BoardData[toRankIndex, toFileIndex];
            state.BoardData[fromRankIndex, fromFileIndex] = string.Empty;
            state.BoardData[toRankIndex, toFileIndex] = movingPiece;
            if (state.BlackCanMovePiece)
            {
                state.BlackPieceStand.Add(pieceToBeTaken.Replace("+", "").ToUpper());
            }
            else
            {
                state.WhitePieceStand.Add(pieceToBeTaken.Replace("+", "").ToLower());
            }
        }
        else if (from is PieceStandPosition ps)
        {
            if (state.BoardData[toRankIndex, toFileIndex] != string.Empty)
            {
                throw new System.Exception("can't set piece on opponent's piece");
            }

            state.BoardData[toRankIndex, toFileIndex] = ps.PieceChar;
            if (state.BlackCanMovePiece)
            {
                state.BlackPieceStand.Remove(ps.PieceChar);
            }
            else
            {
                state.WhitePieceStand.Remove(ps.PieceChar);
            }
        }
        else
        {
            throw new System.Exception("Unexpected Operation");
        }
    }

    public void NextTurn(GameState gs)
    {
        gs.BlackCanMovePiece = !gs.BlackCanMovePiece;
    }

    private static bool IsValidPieceChar(char ch)
    {
        return ch switch
        {
            'p' or 'l' or 'n' or 's' or 'g' or 'k' or 'b' or 'r' => true,
            'P' or 'L' or 'N' or 'S' or 'G' or 'K' or 'B' or 'R' => true,
            _ => false
        };
    }

    public void SetPromotionLine(int bpl, int wpl)
    {
        BlackPromotionLine = bpl;
        WhitePromotionLine = wpl;
    }

    public bool CanPromote(Tile from, Tile to, GameState gs)
    {
        bool canPromote = gs.BlackCanMovePiece
            ? (from.Ranks <= BlackPromotionLine) || (to.Ranks <= BlackPromotionLine)
            : (from.Ranks >= WhitePromotionLine) || (to.Ranks >= WhitePromotionLine);

        return canPromote;
    }
}
