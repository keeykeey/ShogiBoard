using System;
using Avalonia.Controls;
using ShogiBoard.ViewModels;

namespace ShogiBoard.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closed += OnCloseMainWindow;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }

    private void OnCloseMainWindow(object? sender, EventArgs arg)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.GameViewModel.KillEngine();
        }
    }

    private void OnUnhandledException(object? sender, EventArgs arg)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.GameViewModel.KillEngine();
        }
    }
}