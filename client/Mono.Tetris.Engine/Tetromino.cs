namespace Mono.Tetris.Engine;

public class Tetromino
{
    private static readonly List<Func<Tetromino>> TetrominoPool = new List<Func<Tetromino>>
    {
        CreateSquareTetromino,
        CreateLineTetromino,
        CreateTTetromino,
        CreateLTetromino,
        CreateZTetromino
    };

    public static Tetromino CreateRandomTetromino()
    {
        Random random = new Random();
        int randomIndex = random.Next(TetrominoPool.Count);
        return TetrominoPool[randomIndex]();
    }

    public Position? CurrentPosition { get; set; }
    public List<Cell> Cells { get; }
    public bool IsOnGrid => CurrentPosition != null;

    public Color Color;

    public Tetromino(List<Cell> cells, Color color)
    {
        Cells = cells;
        Color = color;
    }

    public void SetOnGrid(Position position)
    {
        CurrentPosition = new Position(position);
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

    public void RotateClockwise()
    {
        foreach (var cell in Cells)
        {
            // Appliquer la transformation de rotation (x', y') = (-y, x)
            int newX = -cell.Position.Y;
            int newY = cell.Position.X;

            // Mettre à jour la position de chaque cellule
            cell.Position = new Position(newX, newY);
        }
    }

    public void RotateCounterClockwise()
    {
        foreach (var cell in Cells)
        {
            // Appliquer la transformation de rotation (x', y') = (y, -x)
            int newX = cell.Position.Y;
            int newY = -cell.Position.X;

            // Mettre à jour la position de chaque cellule
            cell.Position = new Position(newX, newY);
        }
    }

    #region Factory Methods

    public static Tetromino CreateSquareTetromino()
    {
        var square = new List<Cell>
        {
            new(0, 0, true), new(0, 1, true), new(1, 0, true), new(1, 1, true),
        };
        return new Tetromino(square, new Color("Orange"));
    }

    // Pièce en ligne (4x1)
    public static Tetromino CreateLineTetromino()
    {
        var line = new List<Cell>
        {
            new(0, 0, true), new(0, 1, true), new(0, 2, true), new(0, 3, true),
        };
        return new Tetromino(line, new Color("Red"));
    }

    // Pièce en T
    public static Tetromino CreateTTetromino()
    {
        var tShape = new List<Cell>
        {
            new(1, 0, true), new(0, 1, true), new(1, 1, true), new(2, 1, true),
        };
        return new Tetromino(tShape, new Color("Green"));
    }

    // Pièce en L
    public static Tetromino CreateLTetromino()
    {
        var lShape = new List<Cell>
        {
            new(0, 0, true), new(0, 1, true), new(0, 2, true), new(1, 2, true),
        };
        return new Tetromino(lShape, new Color("Blue"));
    }

    // Pièce en Z
    public static Tetromino CreateZTetromino()
    {
        var zShape = new List<Cell>
        {
            new(0, 0, true), new(1, 0, true), new(1, 1, true), new(2, 1, true),
        };
        return new Tetromino(zShape, new Color("Cyan"));
    }

    #endregion
}