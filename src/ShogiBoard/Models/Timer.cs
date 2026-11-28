using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ShogiBoard.Models;

public class TimerStore (
    int byoyomi = 30,
    int timeControlMin = 10,
    int timeControlSec = 0
) {
    public GameTime TimeSetting { get; set; } = new(byoyomi, timeControlMin, timeControlSec);
    public GameTime BlackTimeLeft { get; set; } = new(byoyomi, timeControlMin, timeControlSec);
    public GameTime WhiteTimeLeft { get; set; } = new(byoyomi, timeControlMin, timeControlSec);
    public CancellationTokenSource? TimerCts { get; set; } = new();

    public void Reset()
    {
        BlackTimeLeft.Set(TimeSetting.ByoyomiSec, TimeSetting.TimeControlMin, TimeSetting.TimeControlSec);
        WhiteTimeLeft.Set(TimeSetting.ByoyomiSec, TimeSetting.TimeControlMin, TimeSetting.TimeControlSec);
    }
}

public class GameTime(int byoyomi, int timeControlMin, int timeControlSec) : INotifyPropertyChanged {
    public int ByoyomiSec { get; set; } = byoyomi;
    public int TimeControlMin { get; set; } = timeControlMin;
    public int TimeControlSec { get; set; } = timeControlSec;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Set(int? byoyomi, int? timeControlMin, int? timeControlSec)
    {
        if (byoyomi is int b)
        {
            ByoyomiSec = b;
            NotifyPropertyChanged(nameof(ByoyomiSec));
        }

        if (timeControlMin is int tcm)
        {
            TimeControlMin = tcm;
            NotifyPropertyChanged(nameof(TimeControlMin));
        }

        if (timeControlSec is int tcs)
        {
            TimeControlSec = tcs;
            NotifyPropertyChanged(nameof(TimeControlSec));
        }
    }

    public int ByoyomiMiliSec => ByoyomiSec * 1000;
    public int TimeControlMiliSec => TimeControlMin * 1000 * 60 + TimeControlSec * 1000;
    public bool NoTimeLeft => TimeControlMin == 0 && TimeControlSec == 0;
}