using System;
using System.Diagnostics;
using System.Collections.Generic;
using ShogiBoard.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ShogiBoard.Services;

public interface IEngineCommunicationService
{
    void Send(string m);
    Task<string> SendAsync(string m, Func<string, bool> isResponse, TimeSpan? timeout = null);
    Task StartAsync(string executablePath, Action<string>? responseReceived);
    Task<string> SendSfen(List<Move> moves);
    string CreateSfen(List<Move> moves);
    void Quit();
    void Kill();
    void NewGame();
    void GameWin();
    void GameLose();
    void GameDraw();
}

public class EngineCommunicationService : IEngineCommunicationService
{
    public UsiCommunication Usi { get; set; } = new UsiCommunication();

    private readonly Lock _lock = new();
    private (Func<string, bool> IsResponse, TaskCompletionSource<string> Tcs)? _pending;

    public async Task StartAsync(string executablePath, Action<string>? responseReceived)
    {
        if (Usi.EngineProcess is { HasExited: false }) return;

        var psi = new ProcessStartInfo
        {
            FileName=executablePath,
            UseShellExecute=false,
            RedirectStandardInput=true,
            RedirectStandardOutput=true,
            RedirectStandardError=true,
        };

        Usi.EngineProcess = new Process{ StartInfo=psi, EnableRaisingEvents=true };
        Usi.EngineProcess.OutputDataReceived += (_, args) => OutputDataReceived(args, responseReceived);
        Usi.EngineProcess.Exited += (_, _) =>
        {
            lock (_lock) { _pending?.Tcs.TrySetException(new InvalidOperationException("Engine exited")); }
        };

        Usi.EngineProcess.Start();
        Usi.EngineProcess.BeginOutputReadLine();

        try
        {
            await SendAsync(UsiMessage.USI, response => response == UsiMessage.USI_OK);
            await SendAsync(UsiMessage.IS_READY, response => response == UsiMessage.READY_OK);
        }
        catch
        {
            Kill();
            throw;
        }
    }

    private void OutputDataReceived(DataReceivedEventArgs args, Action<string>? responseReceived)
    {
        if (args.Data is null) return;

        lock (_lock)
        {
            if (_pending is { } p && p.IsResponse(args.Data))
            {
                p.Tcs.TrySetResult(args.Data);
            }
        }
        responseReceived?.Invoke(args.Data);
    }

    public void Send(string m)
    {
        if (Usi.EngineProcess is not { HasExited: false}) return;
        Usi.EngineProcess.StandardInput.WriteLine(m);
    }

    public async Task<string> SendAsync(string m, Func<string, bool>isResponse, TimeSpan? timeout = null)
    {
        if (Usi.EngineProcess is not { HasExited: false })
        {
            throw new InvalidOperationException("Engine is not running");
        }


        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_lock)
        {
            if (_pending is not null) throw new InvalidOperationException("Already waiting for a response");
            _pending = (isResponse, tcs);
        }

        try
        {
            Send(m);
            return await tcs.Task.WaitAsync(timeout ?? TimeSpan.FromSeconds(10));
        }
        finally
        {
            lock (_lock) { _pending = null; }
        }

    }

    public async Task<string> SendSfen(List<Move> moves)
    {
        string sfen = CreateSfen(moves);
        Send(sfen);
        string bestMove = await SendAsync(
            $"{UsiMessage.GO} btime 0 wtime 0 byoyomi 3000",  // TODO
            response => response.StartsWith(UsiMessage.BEST_MOVE),
            TimeSpan.FromMinutes(1)  // TODO
        );
        return bestMove;
    }

    public string CreateSfen(List<Move> _moves)
    {
        IEnumerable<string> moves = _moves.Select(m => m.ToString());
        string sfen = UsiMessage.SFEN_BASE_TEXT + string.Join(" ", moves);
        Console.WriteLine(sfen);
        return sfen;
    }

    public void Quit()
    {
        Send(UsiMessage.QUIT);
    }

    public void Kill()
    {
        if (Usi.EngineProcess is { HasExited: false})
        {
            Usi.EngineProcess.Kill();
            Console.WriteLine("Engine process is killed");
        }
    }

    public void NewGame()
    {
        Send(UsiMessage.NEW_GAME);
    }

    public void GameWin()
    {
        Send($"{UsiMessage.GAME_OVER} {UsiMessage.WIN}");
    }

    public void GameLose()
    {
        Send($"{UsiMessage.GAME_OVER} {UsiMessage.LOSE}");
    }

    public void GameDraw()
    {
        Send($"{UsiMessage.GAME_OVER} {UsiMessage.DRAW}");
    }
}