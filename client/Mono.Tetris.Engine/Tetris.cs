namespace Mono.Tetris.Engine;

public class Tetris
{
    public string PlayerName;
    
    private Tetromino? _currentTetromino;
    public Tetromino? CurrentTetromino => _currentTetromino;
    private Position _startPosition { get; }
    
    public Action<Tetromino> OnTetrominoAdded;

    private Grid _grid;
    public Grid Render() => _grid;

    public Tetris(int rows, int columns, string playerName)
    {
        PlayerName = playerName;
        // O-based index
        _startPosition = new Position(columns / 2, rows);
        _grid = new Grid(rows, columns);
    }

    public void AddTetromino(Tetromino tetromino)
    {
        tetromino.SetOnGrid(_startPosition);
        _currentTetromino = tetromino;
        
        OnTetrominoAdded?.Invoke(tetromino);
    }

    public void Play()
    {
        if (_currentTetromino == null)
            return;

        var oldCells = _currentTetromino!.GetAbsolutePositions();
        _grid.Free(oldCells);
        if (_grid.CanMoveDown(_currentTetromino))
        {
            FallDown();
        }
        else
        {
            _grid.Fill(oldCells, _currentTetromino.Color);
            _grid.CheckForCompletedLines();
            AddTetromino(Tetromino.CreateSquareTetromino());
        }

        var newCells = _currentTetromino.GetAbsolutePositions();
        _grid.Fill(newCells, _currentTetromino.Color);
    }

    private void FallDown()
    {
        _currentTetromino!.CurrentPosition!.Add(new Position(0, -1));
    }

    public void DropTetromino()
    {
        // Libérer les anciennes positions
        var oldCells = _currentTetromino!.GetAbsolutePositions();
        _grid.Free(oldCells);

        // Faire tomber le tetromino jusqu'à collision
        while (_grid.CanMoveDown(_currentTetromino))
        {
            FallDown();
        }

        // Remplir la grille avec la position finale du tetromino
        var finalCells = _currentTetromino.GetAbsolutePositions();
        _grid.Fill(finalCells, _currentTetromino.Color);
        
        _grid.CheckForCompletedLines();
        // Le tetromino est maintenant fixé, on génère un nouveau tetromino
        _currentTetromino = null;
        AddTetromino(Tetromino.CreateRandomTetromino());
    }

    public void MoveTetromino(int x, int y)
    {
        if (_currentTetromino == null)
            return;

        // Libérer les anciennes positions du tetromino
        var oldCells = _currentTetromino.GetAbsolutePositions();
        _grid.Free(oldCells);

        // Appliquer le déplacement
        _currentTetromino.CurrentPosition!.Add(new Position(x, y));

        // Vérifier si le mouvement est possible (pas de collision)
        if (_grid.Collide(_currentTetromino))
        {
            // Si collision, annuler le déplacement
            _currentTetromino.CurrentPosition!.Add(new Position(-x, -y));
        }

        // Remplir la grille avec les nouvelles positions
        var newCells = _currentTetromino.GetAbsolutePositions();
        _grid.Fill(newCells, _currentTetromino.Color);
    }

    public void RotateTetromio()
    {
        // Libérer les anciennes positions du tetromino
        var oldCells = _currentTetromino.GetAbsolutePositions();
        _grid.Free(oldCells);

        // Appliquer la rotation
        _currentTetromino.RotateClockwise();

        // Vérifier si le mouvement est possible (pas de collision)
        if (_grid.Collide(_currentTetromino))
        {
            _currentTetromino.RotateCounterClockwise();
        }

        // Remplir la grille avec les nouvelles positions
        var newCells = _currentTetromino.GetAbsolutePositions();
        _grid.Fill(newCells, _currentTetromino.Color);
    }
}