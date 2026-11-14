using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ShogiBoard.ViewModels;

namespace ShogiBoard.Views.Parts;

public partial class UserOperation : UserControl
{
    public UserOperation()
    {
        InitializeComponent();
    }

    public async void SelectEngine(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Engine",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            string path = files[0].Path.AbsolutePath;
            if (DataContext is GameViewModel vm)
            {
                vm.SetEnginePath(path);
            }
        }
    }
}