using System.Collections.Generic;

namespace ShogiBoard.Models;

public interface IGameRecord
{
    IGameRecord? GetPrev();
    IGameRecord? GetNext();
    void SetPrev(IGameRecord prev);
    void LinkNext(IGameRecord next);
}

public class GameRecordHead : IGameRecord
{
    private IGameRecord? Next { get; set; } = null;
    private IGameRecord? Prev { get; set; } = null;

    public IGameRecord? GetPrev() => Prev;
    public IGameRecord? GetNext() => Next;

    public void SetPrev(IGameRecord prev) { /** void */}

    public void LinkNext(IGameRecord next)
    {
        next.SetPrev(this);
        Next = next;
    }
}

public class GameRecordTail : IGameRecord
{
    private IGameRecord? Next { get; set; } = null;
    private IGameRecord? Prev { get; set; } = null;

    public IGameRecord? GetPrev() => Prev;
    public IGameRecord? GetNext() => Next;

    public void SetPrev(IGameRecord prev)
    {
        Prev = prev;
    }

    public void LinkNext(IGameRecord next) { /** void */}
}

public class GameRecord(
    Move move,
    string pieceTaked = "",
    int? score = null,
    int? byoyomiBlack = null,
    int? timeControlMinBlack = null,
    int? timeControlSecBlack = null,
    int? byoyomiWhite = null,
    int? timeControlMinWhite = null,
    int? timeControlSecWhite = null
) : IGameRecord {
    public Move Move { get; } = move;
    public string PieceTaked { get; } = pieceTaked;
    public int? Score { get; } = score;
    public int? ByoyomiBlack { get; } = byoyomiBlack;
    public int? TimeControlMinBlack { get; } = timeControlMinBlack;
    public int? TimeControlSecBlack { get; } = timeControlSecBlack;
    public int? ByoyomiWhite { get; } = byoyomiWhite;
    public int? TimeControlMinWhite { get; } = timeControlMinWhite;
    public int? TimeControlSecWhite { get; } = timeControlSecWhite;
    private IGameRecord? Next { get; set; } = null;
    private IGameRecord? Prev { get; set; } = null;
    private List<GameRecord> BranchNext { get; set; } = [];

    public IGameRecord? GetNext() => Next;
    public void LinkNext(IGameRecord next)
    {
        next.SetPrev(this);
        Next = next;
    }

    public IGameRecord? GetPrev() => Prev;
    public void SetPrev(IGameRecord prev)
    {
        Prev = prev;
    }

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
}

public class GameRecordStore
{
    public readonly GameRecordHead GameRecordHead = new();
    public readonly GameRecordTail GameRecordTail = new();
    public IGameRecord GameRecordCurrent { get; set; }
    public GameRecordStore()
    {
        GameRecordHead.LinkNext(GameRecordTail);
        GameRecordCurrent = GameRecordHead;
    }

    public void Reset()
    {
        GameRecordHead.LinkNext(GameRecordTail);
        GameRecordCurrent = GameRecordHead;
    }

    public void LinkNext(GameRecord gr)
    {
        gr.LinkNext(GameRecordTail);
        GameRecordCurrent.LinkNext(gr);
        GameRecordCurrent = gr;
    }

    public IEnumerable<Move> Moves()
    {
        IGameRecord? runner = GameRecordHead;
        int safeGuard = 500;
        while(runner is not null && --safeGuard >= 0)
        {
            if (runner is GameRecord gm)
            {
                yield return gm.Move;
            }
            runner = runner.GetNext();
        }
    }
}