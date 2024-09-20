namespace Mono.Tetris.Engine;

public class Cell
{
    public Position Position { get; set; }
    public Color Color { get; set; }
    public bool Filled { get; set; }
    
    public Cell(int x, int y, bool filled = false)
    {
        Position = new Position(x, y);
        Color = new Color("Grey");
        Filled = filled;
    }
}