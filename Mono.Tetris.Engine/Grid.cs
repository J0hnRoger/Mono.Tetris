namespace Mono.Tetris.Engine;

public class Grid
{
    public readonly int Rows;
    public readonly int Columns;
    public List<Cell> Cells { get; }

    public Grid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;

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

    public bool CanMoveDown(Tetromino tetromino)
    {
        foreach (var pos in tetromino.GetAbsolutePositions())
        {
            // Vérifie si le tetromino touche le bas de la grille
            if (pos.Y == 0)
                return false;

            // Vérifie si une cellule en dessous est occupée
            var cellBelow = GetCell(new Position(pos.X, pos.Y));
            if (cellBelow != null && cellBelow.Filled)
                return false;
        }

        return true;
    }

    public bool Collide(Tetromino tetromino)
    {
        return false;
    }
}