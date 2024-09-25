namespace Mono.Tetris.Engine;

public class TetrominoFactory : ITetrominoFactory
{
    
}

public interface ITetrominoFactory {
    
    public Tetromino GetRandom()
    {
        return Tetromino.CreateRandomTetromino();
    }
}