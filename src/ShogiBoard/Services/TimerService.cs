using System;
using System.Threading;
using ShogiBoard.Models;
using ShogiBoard.Helper;
using System.Threading.Tasks;

namespace ShogiBoard.Services;


public interface ITimerService
{
    void StopTimer(TimerStore timerStore);
    void RestartTimer(TimerStore timerStore, bool turnIsBlack, Func<Task> timeLoseCallback);
}

class TimerService : ITimerService
{
    public void StopTimer(TimerStore ts)
    {
        if (ts.TimerCts is null) return;
        ts.TimerCts.Cancel();
        ts.TimerCts.Dispose();
        ts.TimerCts = null;
    }

    public void RestartTimer(TimerStore ts, bool turnIsBlack, Func<Task> timeLoseCallback)
    {
        StopTimer(ts);
        ts.TimerCts = new CancellationTokenSource();
        CountGameTime(ts, turnIsBlack, timeLoseCallback).Background();
    }

    private static async Task CountGameTime(TimerStore ts, bool blackCanMovePiece, Func<Task> timeLoseCallback)
    {
        GameTime timeLeft = blackCanMovePiece ? ts.BlackTimeLeft : ts.WhiteTimeLeft;
        timeLeft.Set(
            byoyomi:        ts.TimeSetting.ByoyomiSec,
            timeControlMin: timeLeft.TimeControlMin,
            timeControlSec: timeLeft.TimeControlSec
        );

        if (timeLeft.NoTimeLeft)
        {
            if (ts.TimerCts is null) return;
            bool ok = await CountByoyomi(timeSetting: ts.TimeSetting, timeLeft: timeLeft, cancellationToken: ts.TimerCts.Token);
            if (!ok)
            {
                await timeLoseCallback.Invoke();
            }
        }
        else
        {
            if (ts.TimerCts is null) return;
            await CountTimeControl(timeLeft: timeLeft, cancellationToken: ts.TimerCts.Token);
            if (timeLeft.NoTimeLeft)
            {
                bool ok = await CountByoyomi(timeSetting: ts.TimeSetting, timeLeft: timeLeft, cancellationToken: ts.TimerCts.Token);
                if (!ok)
                {
                    await timeLoseCallback.Invoke();
                }
            }
            else
            {
                throw new Exception("Unexpected behavior of CountGameTime");
            }
        }
    }

    private static async Task<bool> CountByoyomi(GameTime timeSetting, GameTime timeLeft, CancellationToken cancellationToken)
    {
        int c = timeSetting.ByoyomiSec;
        for (int i = 0; i < c; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            timeLeft.Set(
                byoyomi: timeLeft.ByoyomiSec - 1,
                timeControlMin: null,
                timeControlSec: null
            );
            await Task.Delay(1000, cancellationToken);
        }

        return false;
    }

    private static async Task CountTimeControl(GameTime timeLeft, CancellationToken cancellationToken)
    {
        int c = timeLeft.TimeControlMin * 60 + timeLeft.TimeControlSec;
        for (int i = 0; i < c; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int byou = timeLeft.TimeControlSec == 0 ? 59 : timeLeft.TimeControlSec - 1;
            int min = byou == 59 ? timeLeft.TimeControlMin - 1 : timeLeft.TimeControlMin;
            timeLeft.Set(
                byoyomi: timeLeft.ByoyomiSec,
                timeControlMin: min,
                timeControlSec: byou
            );
            await Task.Delay(1000, cancellationToken);
        }
    }
}