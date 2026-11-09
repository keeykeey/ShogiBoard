using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using ShogiBoard.Models;
using ShogiBoard.ViewModels;

namespace ShogiBoard.Views.Parts;

public partial class PieceStandView : UserControl
{
    public static readonly StyledProperty<IEnumerable<PieceOnStand>?> PiecesProperty =
        AvaloniaProperty.Register<PieceStandView, IEnumerable<PieceOnStand>?>(nameof(Pieces));

    public IEnumerable<PieceOnStand>? Pieces
    {
        get => GetValue(PiecesProperty);
        set => SetValue(PiecesProperty, value);
    }

    public PieceStandView()
    {
        InitializeComponent();
    }

    private async void OnPiecePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Image img) return;
        string? pieceName =  img.Name;
        if (pieceName is null) return;

        if (DataContext is GameViewModel vm)
        {
            vm.ClickPieceStand($"{pieceName}*");
        }
    }
}
