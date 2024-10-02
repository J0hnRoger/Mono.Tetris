namespace Mono.Tetris.Engine;

public abstract class TetrisAction
{
    public override string ToString()
    {
        return $"Action: {GetType().Name}";
    }

    public abstract void Execute(Tetris tetrisGame);
}

public class MoveAction : TetrisAction
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public MoveAction(int x, int y)
    {
       X = x; 
    }

    public override void Execute(Tetris tetrisGame)
    {
        tetrisGame.MoveTetromino(X, Y);
    }
}

public class RotateAction : TetrisAction
{
    public override void Execute(Engine.Tetris tetrisGame)
    {
        tetrisGame.RotateTetromio();
    }
}

public class DropAction : TetrisAction
{
    public override void Execute(Engine.Tetris tetrisGame)
    {
        tetrisGame.DropTetromino();
    }
}
