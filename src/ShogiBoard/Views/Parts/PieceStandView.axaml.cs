using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using ShogiBoard.Models;

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
}
