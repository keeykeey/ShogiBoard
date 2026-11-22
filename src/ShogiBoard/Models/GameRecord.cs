using System.Collections.Generic;

namespace ShogiBoard.Models;

public class GameRecord(
    Move move,
    int? score = null,
    int? byoyomiBlack = null,
    int? timeControlMinBlack = null,
    int? timeControlSecBlack = null,
    int? byoyomiWhite = null,
    int? timeControlMinWhite = null,
    int? timeControlSecWhite = null)
{
    public Move Move { get; } = move;
    public int? Score { get; } = score;
    public int? ByoyomiBlack { get; } = byoyomiBlack;
    public int? TimeControlMinBlack { get; } = timeControlMinBlack;
    public int? TimeControlSecBlack { get; } = timeControlSecBlack;
    public int? ByoyomiWhite { get; } = byoyomiWhite;
    public int? TimeControlMinWhite { get; } = timeControlMinWhite;
    public int? TimeControlSecWhite { get; } = timeControlSecWhite;
    private GameRecord? Next { get; set; } = null;
    private GameRecord? Prev { get; set; } = null;
    private List<GameRecord> BranchNext { get; set; } = [];

    public GameRecord? GetNext() => Next;
    public void SetNext(GameRecord next)
    {
        next.Prev = this;
        Next = next;
    }

    public GameRecord? GetPrev() => Prev;

    public GameRecord? GetBranchNext(int branchIndex)
    {
        if (branchIndex < 0 || branchIndex >= BranchNext.Count) throw new System.ArgumentException($"Index out of range ({branchIndex})");

        return BranchNext[branchIndex];
    }
    public void SetBranchNext(GameRecord next)
    {
        next.Prev = this;
        BranchNext.Add(next);
    }

    public IEnumerable<Move> Moves()
    {
        GameRecord? runner = this;
        int safeGuard = 500;
        while(runner is not null && --safeGuard >= 0)
        {
            yield return runner.Move;
            runner = runner.Next;
        }
    }
}
