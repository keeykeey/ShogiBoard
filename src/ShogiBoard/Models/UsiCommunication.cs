using System.Diagnostics;
using System.Collections.Generic;

namespace ShogiBoard.Models;

public class UsiCommunication
{
    public Process? EngineProcess { get; set; }
    public Stack<string> ResponseStacks = [];
}