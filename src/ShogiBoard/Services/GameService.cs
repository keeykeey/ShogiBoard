using ShogiBoard.Models;

namespace ShogiBoard.Services;

public interface IGameService
{
    void Init9DimBoardData(GameState gameState);
    void SetBoardData(GameState gameState, string sfenBoardText);
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
        SetBoardData(gameState, startPos9Dim);
    }

    public void SetBoardData(GameState gameState, string sfenBoardText)
    {
        for (int i = 0, file = gameState.Board.NFile, rank = 1 ; i < sfenBoardText.Length; ++i )
        {
            char ch = sfenBoardText[i];
            if (ch == '/')
            {
                file = gameState.Board.NFile;
                ++rank;
                if (gameState.Board.NRank < rank)
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
                gameState.Board.Set(file: file, rank: rank, piece: s);
                i += 1;
                --file;
            }
            else if (int.TryParse(ch.ToString(), out var n))
            {
                if (n < 0 || file < n) throw new System.Exception($"Unexpected character at position ({i}) from {sfenBoardText}. Got {n}");

                for (int _n = 0; _n < n; _n++)
                {
                    gameState.Board.Set(file: file - _n, rank: rank, piece: string.Empty);
                }

                file -= n;
            }
            else if (IsValidPieceChar(ch))
            {
                gameState.Board.Set(file: file, rank: rank, piece: ch.ToString());
                --file;
            }
            else
            {
                throw new System.Exception($"Unexpected character at position ({i}) from {sfenBoardText}. Got ({ch})");
            }

            if (gameState.Board.NFile < file - 1)
            {
                throw new System.Exception($"Too long width({file}) for the board(rank: {gameState.Board.NRank}, file: {gameState.Board.NFile}). (index: {i}, ch: {ch})");
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
        if (from is Tile tile)
        {
            string doPromoteStr = promote ? "+" : "";
            string movingPiece = $"{doPromoteStr}{state.Board.Get(rank: tile.Ranks, file: tile.Files)}";
            string pieceToBeTaken = state.Board.Get(rank: to.Ranks, file: to.Files);
            state.Board.Set(rank: tile.Ranks, file: tile.Files, string.Empty);
            state.Board.Set(rank: to.Ranks, file: to.Files, piece: movingPiece);
            if (WhiteOwns(pieceToBeTaken))
            {
                state.BlackPieceStand.Add(pieceToBeTaken.Replace("+", "").ToUpper());
            }
            else if (BlackOwns(pieceToBeTaken))
            {
                state.WhitePieceStand.Add(pieceToBeTaken.Replace("+", "").ToLower());
            }
        }
        else if (from is PieceStandPosition ps)
        {
            if (state.Board.Get(rank: to.Ranks, file: to.Files) != string.Empty)
            {
                throw new System.Exception("can't set piece on opponent's piece");
            }

            state.Board.Set(rank: to.Ranks, file: to.Files, ps.PieceChar);
            if (BlackOwns(ps.PieceChar))
            {
                state.BlackPieceStand.Remove(ps.PieceChar);
            }
            else if (WhiteOwns(ps.PieceChar))
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
        if (gs.InAGame)
        {
            gs.BlackCanMovePiece = !gs.BlackCanMovePiece;
        }
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
        string p = gs.Board.Get(rank: from.Ranks, file: from.Files) ?? " ";
        if (BlackOwns(p) && p[0] != '+')
        {
            return (from.Ranks <= BlackPromotionLine) || (to.Ranks <= BlackPromotionLine);
        }
        else if (WhiteOwns(p) && p[0] != '+')
        {
            return (from.Ranks >= WhitePromotionLine) || (to.Ranks >= WhitePromotionLine);
        }
        else
        {
            return false;
        }
    }

    private static bool BlackOwns(string piece)
    {
        if (piece == string.Empty) return false;
        return char.IsUpper(piece[0]) || (piece.Length == 2 && piece[0].Equals('+') && char.IsUpper(piece[1]));
    }

    private static bool WhiteOwns(string piece)
    {
        if (piece == string.Empty) return false;
        return char.IsLower(piece[0]) || (piece.Length == 2 && piece[0].Equals('+') && char.IsLower(piece[1]));
    }
}
