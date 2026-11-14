using System.Threading.Tasks;
using System;

namespace ShogiBoard.Helper;

public static class TaskExtensions
{
    // extention to execute asynchronous task background.
    // usage:
    //   define the async function FooTask()
    //   and execute Footask().Background()
    public static async void Background(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"asynchronous function failed. {ex.Message}");
        }
    }
}