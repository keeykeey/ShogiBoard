using System;
using System.Diagnostics;
using ShogiBoard.Models;

namespace ShogiBoard.Services;

public interface IEngineCommunicationService
{
    event Action<string>? ResponseReceived;
    void Start(string excutablePath);
    void Send(string m);
    void Quit();
    void Kill();
}

public class EngineCommunicationService : IEngineCommunicationService
{
    public UsiCommunication Usi { get; set; } = new UsiCommunication();
    public event Action<string>? ResponseReceived;

    public void Start(string excutablePath)
    {
        if (Usi.EngineProcess is { HasExited: false })
        {
            return;
        }

        var psi = new ProcessStartInfo
        {
            FileName=excutablePath,
            UseShellExecute=false,
            RedirectStandardInput=true,
            RedirectStandardOutput=true,
            RedirectStandardError=true,
        };

        Usi.EngineProcess = new Process{ StartInfo=psi, EnableRaisingEvents=true };
        Usi.EngineProcess.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                ResponseReceived?.Invoke(args.Data);
            }
        };

        Usi.EngineProcess.Start();
        Usi.EngineProcess.BeginOutputReadLine();
    }

    public void Send(string m)
    {
        if (Usi.EngineProcess is { HasExited: false})
        {
            Usi.EngineProcess.StandardInput.WriteLine(m);
        }
    }

    public void Quit()
    {
        Send("quit");
    }

    public void Kill()
    {
        if (Usi.EngineProcess is { HasExited: false})
        {
            Usi.EngineProcess.Kill();
            Console.WriteLine("Engine process is killed");
        }
    }
}