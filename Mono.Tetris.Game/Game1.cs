using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono.Tetris.Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _cellTexture;

    private double _timeSinceLastUpdate;

    private readonly Engine.Tetris _tetrisGame;
    private const double _updateInterval = 0.5;

    private int cellSize = 20;
    private int columns = 20;
    private int rows = 20;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _tetrisGame = new Engine.Tetris(rows, columns);
        _timeSinceLastUpdate = 0;
    }

    protected override void Initialize()
    {
        // Calculer la taille nécessaire pour contenir la grille
        int windowWidth = columns * cellSize; // Largeur de la fenêtre en fonction des colonnes
        int windowHeight = rows * cellSize; // Hauteur de la fenêtre en fonction des lignes

        // Définir la taille de la fenêtre
        _graphics.PreferredBackBufferWidth = windowWidth;
        _graphics.PreferredBackBufferHeight = windowHeight;
        _graphics.ApplyChanges(); // Appliquer les modifications

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _cellTexture = new Texture2D(GraphicsDevice, 1, 1);
        _cellTexture.SetData(new[] {Color.White});
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        _timeSinceLastUpdate += deltaTime;

        if (_timeSinceLastUpdate >= _updateInterval)
        {
            _tetrisGame.Play();
            _timeSinceLastUpdate = 0; // Réinitialiser le compteur
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Nettoyer l'écran
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Démarrer le batch de dessin
        _spriteBatch.Begin();

        // Récupérer la grille
        var grid = _tetrisGame.Render();

        // Taille d'une cellule à l'écran (exemple de 20 pixels par 20 pixels)
        int cellSize = 20;

        // Parcourir chaque cellule de la grille
        foreach (var cell in grid.Cells)
        {
            // Calculer la position sur l'écran
            int screenX = cell.Position.X * cellSize;
            int screenY = (grid.Rows - cell.Position.Y) * cellSize;

            // Choisir une couleur pour la cellule (par exemple, Color.Gray pour les cellules vides)
            Color color = cell.Filled ? Color.Red : Color.Gray;

            // Dessiner la cellule (rectangle coloré)
            _spriteBatch.Draw(_cellTexture, new Rectangle(screenX, screenY, cellSize, cellSize), color);
        }

        // Finir le batch de dessin
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}