using System;
using System.IO;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShogiBoard.Models;
using ShogiBoard.Services;

namespace ShogiBoard.ViewModels;

public partial class GameViewModel : ViewModelBase
{
    public GameViewModel(IEngineCommunicationService ecs, IGameService gs)
    {
        EngineCommunicator = ecs;
        EngineCommunicator.ResponseReceived += ResponseReceived;
        GameService = gs;
    }

    public IEngineCommunicationService EngineCommunicator { get; }
    public IGameService GameService { get; }

    [ObservableProperty]
    public partial string Response { get; set; } = "";

    [ObservableProperty]
    public partial string Message { get; set; } = "";

    [ObservableProperty]
    public partial string Sfen { get; set; } = "";

    [RelayCommand]
    private void StartEngine()
    {
        string excutablePath = Path.Combine(AppContext.BaseDirectory, "engine", "Lesserkai");
        EngineCommunicator.Start(excutablePath);
    }

    [RelayCommand]
    private void SendMessage()
    {
        EngineCommunicator.Send(Message);
    }

    [RelayCommand]
    private void QuitEngine()
    {
        EngineCommunicator.Quit();
    }

    private void ResponseReceived(string m)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Response = m;
            Sfen = m;
        });
    }

    public void KillEngine()
    {
        EngineCommunicator.Kill();
    }

    public void ClickBoard(int col, int row)
    {
        var point = new Point2D{X = col, Y = row};
        GameService.ClickBoard(point);
    }
}

