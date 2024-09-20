using FluentAssertions;
using Mono.Tetris.Engine;

namespace Mono.Tetris.Tests;

public class GameTests
{
    private Engine.Tetris CreateGame()
    {
        var game = new Engine.Tetris(80, 20);
        return game;
    }

    [Fact]
    public void Game_InitGrid()
    {
        var game = CreateGame();
        game.Should().NotBeNull();
        var gridState = game.Render();
        gridState.Cells.Should().HaveCount(1600);
        gridState.Cells.ForEach(c => c.Filled.Should().BeFalse());
    }

    [Fact]
    public void Game_Render_UpdatedGrid()
    {
        var game = CreateGame();
        var tetromino = CreateSquareTetromino();
        
        game.AddTetromino(tetromino);
        
        game.Play();
        
        var gridState = game.Render();
        
        gridState.Cells.Where(c => c.Filled)
            .Should().HaveCount(2);
    }
    
    [Fact]
    public void Position_Comparison_AsValueObject()
    {
        var position1 = new Position(1, 1);
        var position2 = new Position(1, 1);
        position1.Should().Be(position2);
    }

    private Tetromino CreateSquareTetromino()
    {
        var square = new List<Cell>
        {
            new(0, 0, true),
            new(0, 1, true),
            new(1, 0, true),
            new(1, 1, true),
        };
        return new Tetromino(square, new Color("Purple"));
    }
    
}
