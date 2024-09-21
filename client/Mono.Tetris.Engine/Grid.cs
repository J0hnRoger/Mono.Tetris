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


    public void Fill(List<Position> positions, Color color)
    {
        positions.ForEach(p => SetFilled(p, true, color));
    }

    public void Free(List<Position> positions)
    {
        positions.ForEach(p => SetFilled(p, false, new Color("Grey")));
    }

    public void SetFilled(Position position, bool filled, Color color)
    {
        var cell = GetCell(position);
        //outside of the grid
        if (cell == null)
            return;

        cell.Color = color;
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
            var cellBelow = GetCell(new Position(pos.X, pos.Y - 1));
            if (cellBelow != null && cellBelow.Filled)
                return false;
        }

        return true;
    }

    public bool Collide(Tetromino tetromino)
    {
        foreach (var pos in tetromino.GetAbsolutePositions())
        {
            // Vérification des limites de la grille (X entre 0 et Columns-1, Y entre 0 et Rows-1)
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return true; // Collision avec les bords
            }

            // Vérification des cellules déjà remplies
            var cell = GetCell(pos);
            if (cell != null && cell.Filled)
            {
                return true; // Collision avec une cellule déjà occupée
            }
        }

        return false; // Pas de collision
    }

    public void CheckForCompletedLines()
    {
        for (int y = 0; y < Rows; y++)
        {
            if (IsLineComplete(y))
            {
                ClearLine(y);
                MoveLinesDown(y);
            }
        }
    }

    private void ClearLine(int y)
    {
        // Libérer toutes les cellules de la ligne 'y'
        for (int x = 0; x < Columns; x++)
        {
            var cell = GetCell(new Position(x, y));
            if (cell != null)
            {
                cell.Filled = false; // Libérer la cellule
            }
        }
    }

    private void MoveLinesDown(int startY)
    {
        // Déplacer toutes les lignes au-dessus de la ligne 'startY' vers le bas
        for (int y = startY; y < Rows - 1; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                var currentCell = GetCell(new Position(x, y));
                var aboveCell = GetCell(new Position(x, y + 1));

                if (currentCell != null && aboveCell != null)
                {
                    // Copier l'état de la cellule au-dessus
                    currentCell.Filled = aboveCell.Filled;
                    currentCell.Color = aboveCell.Color; // Si tu as une couleur par cellule
                }
            }
        }

        // Vider la première ligne (tout en haut)
        for (int x = 0; x < Columns; x++)
        {
            var cell = GetCell(new Position(x, Rows - 1));
            if (cell != null)
            {
                cell.Filled = false;
            }
        }
    }

    private bool IsLineComplete(int y)
    {
        // Vérifier si toutes les cellules de la ligne 'y' sont remplies
        for (int x = 0; x < Columns; x++)
        {
            var cell = GetCell(new Position(x, y));
            if (cell == null || !cell.Filled)
            {
                return false; // Si une cellule n'est pas remplie, la ligne n'est pas complète
            }
        }

        return true; // Toutes les cellules sont remplies, la ligne est complète
    }
}