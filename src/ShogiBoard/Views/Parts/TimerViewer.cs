using Avalonia;
using Avalonia.Controls;
using ShogiBoard.Models;

namespace ShogiBoard.Views.Parts;

public partial class TimerViewer : UserControl
{
    public static readonly StyledProperty<GameTime> TimerProperty =
        AvaloniaProperty.Register<TimerViewer, GameTime>(nameof(Timer));

    public GameTime Timer
    {
        get => GetValue(TimerProperty);
        set => SetValue(TimerProperty, value);
    }

    public TimerViewer()
    {
        InitializeComponent();
    }
}
