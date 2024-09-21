using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Tetris.Game.Common;
using Mono.Tetris.Game.Extensions;

namespace Mono.Tetris.Game.Screens;

public class GameScreen : IScreen
{
    private Engine.Tetris _tetrisGame;
    private double _timeSinceLastUpdate;
    private const double _updateInterval = 0.5;

    private KeyboardState _previousKeyboardState;
    private SpriteBatch _spriteBatch;
    private Texture2D _cellTexture;
    private int cellSize = 20;
    private int columns = 20;
    private int rows = 20;

    public GameScreen(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        _tetrisGame = new Engine.Tetris(rows, columns);
        _spriteBatch = spriteBatch;
        _cellTexture = new Texture2D(graphicsDevice, 1, 1);
        _cellTexture.SetData(new[] {Color.White});
    }

    public void Update(GameTime gameTime)
    {
        double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        _timeSinceLastUpdate += deltaTime;

        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _tetrisGame.RotateTetromio();
        }

        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            _tetrisGame.MoveTetromino(-1, 0); // Déplacement vers la gauche
        }

        // Déplacer à droite
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            _tetrisGame.MoveTetromino(1, 0); // Déplacement vers la droite
        }

        // Accélérer la chute
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _tetrisGame.MoveTetromino(0, -1); // Déplacement vers le bas
        }

        // Faire tomber la pièce directement (optionnel)
        if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
        {
            _tetrisGame.DropTetromino(); // Méthode à implémenter pour faire tomber la pièce directement
        }

        if (_timeSinceLastUpdate >= _updateInterval)
        {
            _tetrisGame.Play();
            _timeSinceLastUpdate = 0; // Réinitialiser le compteur
        }

        _previousKeyboardState = keyboardState;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        var grid = _tetrisGame.Render();

        foreach (var cell in grid.Cells)
        {
            int screenX = cell.Position.X * cellSize;
            int screenY = (grid.Rows - cell.Position.Y - 1) * cellSize;
            Color color = cell.Filled ? cell.Color.GetMonoGameColorByName() ?? Color.Gray : Color.Gray;

            spriteBatch.Draw(_cellTexture, new Rectangle(screenX, screenY, cellSize, cellSize), color);
        }

        spriteBatch.End();
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
    }
}