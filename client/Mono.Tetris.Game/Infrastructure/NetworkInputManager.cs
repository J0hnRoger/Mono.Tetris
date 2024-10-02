using System.Collections.Generic;
using System.Diagnostics;
using Mono.Tetris.Engine;

namespace Mono.Tetris.Game.Infrastructure;

public class NetworkInputManager
{
    private Queue<TetrisAction> _networkActions = new();

    public TetrisAction? GetNetworkInput()
    {
        if (_networkActions.Count > 0)
        {
            return _networkActions.Dequeue();
        }
        return null;
    }

    public void ReceiveNetworkMove(string move)
    {
        switch (move)
        {
            case "left":
                _networkActions.Enqueue(new MoveAction(-1, 0)); 
                break;
            case "right":
                _networkActions.Enqueue(new MoveAction(1, 0)); 
                break;
            case "rotate":
                _networkActions.Enqueue(new RotateAction()); 
                break;
            case "down":
                _networkActions.Enqueue(new MoveAction(0, -1)); 
                break;
            case "drop":
                _networkActions.Enqueue(new DropAction()); 
                break;
            default:
                Debug.WriteLine($"Unknown move: {move}");
                break;
        }
    }
}