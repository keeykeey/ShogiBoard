using System;
namespace ShogiBoard.Models;

// promote: 7g7f+
// move from piece stand: P*5e
public struct Move
{
    public IPosition MoveFrom { get; set; }
    public Tile MoveTo { get; set; }
    public bool DoPromote { get; } = false;
    private string DoPromoteChar { get; set; } = "";

    public Move(IPosition moveFrom, Tile moveTo, bool doPromote = false)
    {
        MoveFrom = moveFrom;
        MoveTo = moveTo;
        DoPromote = doPromote;
        DoPromoteChar = doPromote ? "+" : "";
    }

    public Move(string position, bool turnIsBlack)
    {
        if (position.Length < 4) throw new ArgumentException("Unexpected Position for Move");

        string head = position[0].ToString();
        bool moveFromTile = char.IsDigit(head[0]);

        string from = (!turnIsBlack && !moveFromTile)
            ? position.Replace(head, head.ToLower())[..2]  // when white putting piece from piece-stand
            : position[..2];                               // else case
        string to = position.Substring(2, 2);

        if (position.Length == 4)
        {
            if (moveFromTile)
            {
                MoveFrom = new Tile(from);
                MoveTo = new Tile(to);
            }
            else
            {
                MoveFrom = new PieceStandPosition(from);
                MoveTo = new Tile(to);
            }
        }
        else if (position.EndsWith('+') && position.Length == 5)
        {
            MoveFrom = new Tile(from);
            MoveTo = new Tile(to);
            DoPromote = true;
            DoPromoteChar = "+";
        }
        else
        {
            throw new ArgumentException("Unexpected position for Move");
        }
    }

    public override readonly string ToString() =>  $"{MoveFrom.MovePosition()}{MoveTo.MovePosition()}{DoPromoteChar}";
}
