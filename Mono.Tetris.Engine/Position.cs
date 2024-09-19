namespace Mono.Tetris.Engine;

public record Position(int X, int Y)
{
    public int X { get; private set; } = X;
    public int Y { get; private set; } = Y;

    public void Add(Position position)
    {
        X += position.X;
        Y += position.Y;
    }
}