using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Tetris.Engine;
using Mono.Tetris.Game.Common;
using Mono.Tetris.Game.Extensions;
using Mono.Tetris.Game.Infrastructure;
using Color = Microsoft.Xna.Framework.Color;

namespace Mono.Tetris.Game.Screens;

public class GameScreen : IScreen
{
    private readonly SignalRClient _signalRClient;
    private double _timeSinceLastUpdate;
    private const double _updateInterval = 0.5;

    private KeyboardState _previousKeyboardState;
    private SpriteBatch _spriteBatch;
    private Texture2D _cellTexture;
    private int cellSize = 20;
    private int columns = 10;
    private int rows = 20;
    
    private readonly Engine.Tetris _tetrisGame;
    private readonly Engine.Tetris _opponentTetrisGame;

    public GameScreen(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SignalRClient signalRClient)
    {
        _signalRClient = signalRClient;
        _tetrisGame = new Engine.Tetris(rows, columns);
        _tetrisGame.OnTetrominoAdded += TetrominoAdded;
        
        _opponentTetrisGame = new Engine.Tetris(rows, columns);

        _spriteBatch = spriteBatch;
        _cellTexture = new Texture2D(graphicsDevice, 1, 1);
        _cellTexture.SetData(new[] {Color.White});

        _signalRClient.OnMoveReceived(OnOpponentMoveReceived);
        _signalRClient.OnTetrominoReceived(OnTetrominoReceived);
    }

    private void TetrominoAdded(Tetromino tetromino)
    {
        _ = _signalRClient.SendAddTetromino("", tetromino);
    }

    private void OnTetrominoReceived(string playerName, TetrominoDto tetrominoAdded)
    {
        var addedTetromino = Tetromino.FromName(tetrominoAdded.Name);
        _opponentTetrisGame.AddTetromino(addedTetromino);
    }

    public void OnOpponentMoveReceived(string playerName, string move)
    {
        switch (move)
        {
            case "left":
                _opponentTetrisGame.MoveTetromino(-1, 0);
                break;
            case "right":
                _opponentTetrisGame.MoveTetromino(1, 0);
                break;
            case "rotate":
                _opponentTetrisGame.RotateTetromio();
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        _timeSinceLastUpdate += deltaTime;

        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _tetrisGame.RotateTetromio();
            _ = _signalRClient.SendMove("rotate");
        }

        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            _tetrisGame.MoveTetromino(-1, 0); // Déplacement vers la gauche
            _ = _signalRClient.SendMove("left");
        }

        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            _tetrisGame.MoveTetromino(1, 0); // Déplacement vers la droite
            _ = _signalRClient.SendMove("right");
        }

        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _tetrisGame.MoveTetromino(0, -1); // Déplacement vers le bas
        }

        if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
        {
            _tetrisGame.DropTetromino();
        }

        if (_timeSinceLastUpdate >= _updateInterval)
        {
            _tetrisGame.Play();
            _opponentTetrisGame.Play();
            
            _timeSinceLastUpdate = 0;
        }

        _previousKeyboardState = keyboardState;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        Vector2 playerGridPosition = new(0, 0);
        int tetrisGridSize = cellSize * columns;
        int gap = 20;
        Vector2 opponentGridPosition = new(tetrisGridSize + gap, 0);
        
        DrawGrid(_tetrisGame, spriteBatch, playerGridPosition);
        DrawGrid(_opponentTetrisGame, spriteBatch, opponentGridPosition);

        spriteBatch.End();
    }

    private void DrawGrid(Engine.Tetris game, SpriteBatch spriteBatch, Vector2 position)
    {
        var grid = game.Render();

        foreach (var cell in grid.Cells)
        {
            int screenX = (int)(position.X + cell.Position.X * cellSize);
            int screenY = (int)(position.Y + (grid.Rows - cell.Position.Y - 1) * cellSize);
            Color color = cell.Filled ? cell.Color.GetMonoGameColorByName() ?? Color.Gray : Color.Gray;

            spriteBatch.Draw(_cellTexture, new Rectangle(screenX, screenY, cellSize, cellSize), color);
        }
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
    }
}