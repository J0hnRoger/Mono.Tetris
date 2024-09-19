namespace Mono.Tetris.Engine;

public class Tetris
{
    private Tetromino? _currentTetromino;
    private Position _startPosition { get; }
    
    private Grid _grid;
    public Grid Render() => _grid;
    public Tetris(int rows, int columns)
    {
        // O-based index
        _startPosition = new Position(columns / 2, rows);
        _grid = new Grid(rows, columns);
    }

    public void AddTetromino(Tetromino tetromino)
    {
        tetromino.SetOnGrid(_startPosition); 
        _currentTetromino = tetromino;
    }

    public void Play()
    {
        if(_currentTetromino == null)
            AddTetromino(Tetromino.CreateSquareTetromino());
        
        var oldCells = _currentTetromino!.GetAbsolutePositions();
        _grid.Free(oldCells);
       
        if (_grid.CanMoveDown(_currentTetromino))
            FallDown();
        else  
            AddTetromino(Tetromino.CreateSquareTetromino());
        
        var newCells = _currentTetromino.GetAbsolutePositions();
        _grid.Fill(newCells);
    }
    
    private void FallDown()
    {
        _currentTetromino!.CurrentPosition!.Add(new Position(0, -1));
    }
}