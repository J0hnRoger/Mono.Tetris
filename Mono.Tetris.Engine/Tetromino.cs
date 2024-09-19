namespace Mono.Tetris.Engine;

public class Tetromino
{
    public Position? CurrentPosition { get; set; }
    public List<Cell> Cells { get; }
    public bool IsOnGrid => CurrentPosition != null;
    
    public Tetromino(List<Cell> cells)
    {
        Cells = cells;
    }
    
    public void SetOnGrid(Position position)
    {
        CurrentPosition = position;
    }

    public List<Position> GetAbsolutePositions()
    {
        if (!IsOnGrid)
            return new List<Position>();
        
        return Cells
            .Select(c =>
            {
                var newPosition = new Position(c.Position.X + CurrentPosition!.X, c.Position.Y + CurrentPosition.Y);
                return newPosition;
            })
            .ToList();
    } 
}