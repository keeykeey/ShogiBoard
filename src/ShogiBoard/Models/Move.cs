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
    }

    public Move(string position)
    {
        if (position.Length < 4) throw new ArgumentException("Unexpected Position for Move");

        string from = position[..2];
        string to = position.Substring(2, 2);

        if (position.Length == 4)
        {
            if (char.IsDigit(position[0]))
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
