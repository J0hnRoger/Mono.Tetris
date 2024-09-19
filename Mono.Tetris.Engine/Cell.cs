namespace Mono.Tetris.Engine;

public class Cell
{
    public Position Position { get; set; }
    public string Color { get; set; }
    public bool Filled { get; set; }
    
    public Cell(int x, int y, bool filled = false)
    {
        Position = new Position(x, y);
        Color = "FFFFFF";
        Filled = filled;
    }
}