using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ShogiBoard.Services;
using ShogiBoard.ViewModels;
using ShogiBoard.Views;

namespace ShogiBoard;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var service = new ServiceCollection();
        service.AddTransient<IEngineCommunicationService, EngineCommunicationService>();
        service.AddTransient<IGameService, GameService>();
        service.AddTransient<GameViewModel>();
        service.AddTransient<MainViewModel>();

        var provider = service.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}