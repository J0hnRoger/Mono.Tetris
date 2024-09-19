namespace Mono.Tetris.Engine;

public class Grid
{
    private readonly int _rows;
    private readonly int _columns;
    public List<Cell> Cells { get; } 
    public Grid(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
        
        Cells = Enumerable.Range(0, columns)
            .SelectMany(r => Enumerable.Range(0, rows)
                .Select(c => new Cell(r, c)))
            .ToList();
    }

    
    public void Fill(List<Position> positions)
    {
        positions.ForEach(p => SetFilled(p, true));
    }
    
    public void Free(List<Position> positions)
    {
        positions.ForEach(p => SetFilled(p, false));
    }
    
    public void SetFilled(Position position, bool filled)
    {
        var cell = GetCell(position);
        //outside of the grid
        if (cell == null)
            return;
        cell.Filled = filled;
    }
    
    private Cell? GetCell(Position position)
    {
        return Cells.FirstOrDefault(c => c.Position == position);
    }

}