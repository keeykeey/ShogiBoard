
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace ShogiBoard.ViewModels;

public partial class ShowErrorViewModel(
    Window dialog,
    string title,
    string message,
    string onCloseText = "close",
    int height = 200,
    int width = 200
    ) : ViewModelBase
{
    public Window Dialog = dialog;
    public string Title { get; } = title;
    public string Message { get; } = message;
    public string OnCloseText { get; } = onCloseText;
    public int Height { get; } = height;
    public int Width { get; } = width;

    [RelayCommand]
    private void ClickClose() => Dialog.Close(true);
}