using Avalonia.Controls;
using Avalonia.Input;
using ShogiBoard.ViewModels;

namespace ShogiBoard.Views.Dialog;

public partial class AskPromote : Window
{
    public AskPromote()
    {
        InitializeComponent();
    }

    private async void OnPiecePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Image img) return;
        string? pieceName =  img.Name;

        if (DataContext is AskPromoteViewModel vm)
        {
            vm.SelectDoPromote(pieceName);
        }

    }
}
