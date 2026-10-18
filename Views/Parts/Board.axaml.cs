using Avalonia.Controls;
using Avalonia.Input;
using ShogiBoard.ViewModels;
using System;

namespace ShogiBoard.Views.Parts;

public partial class Board : UserControl
{
    // these values were calculated from actual board.png file.
    private const double OrigW = 458, OrigH = 500;  // image's width and height.
    private const double GridLeft = 6, GridTop = 6; // left-space and top-space of the board.
    private const double GridW = 445, GridH = 487;  // width and height of the board.

    public Board()
    {
        InitializeComponent();
    }

    private void OnBoardPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Image img) return;
        var pos = e.GetPosition(img);

        if (pos.X < GridLeft || pos.Y < GridTop) return;

        double scale = Math.Min(img.Bounds.Width / OrigW, img.Bounds.Height / OrigH);
        double offsetX = (img.Bounds.Width  - OrigW * scale) / 2;
        double offsetY = (img.Bounds.Height - OrigH * scale) / 2;

        int col = (int)((pos.X - offsetX - GridLeft * scale) / (GridW * scale / 9));
        int row = (int)((pos.Y - offsetY - GridTop  * scale) / (GridH * scale / 9));

        if (col < 0 || col > 8 || row < 0 || row > 8) return;

        if (DataContext is GameViewModel vm)
            vm.ClickBoard(col, row);
    }
}
