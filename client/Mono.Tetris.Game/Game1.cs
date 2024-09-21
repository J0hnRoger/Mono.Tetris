using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Tetris.Game.Common;
using Mono.Tetris.Game.Infrastructure;
using Mono.Tetris.Game.Screens;

namespace Mono.Tetris.Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private ScreenManager _screenManager;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _cellTexture;
    private SoundEffect _soundEffect;
    private SoundEffectInstance _soundEffectInstance;

    private double _timeSinceLastUpdate;

    private readonly Engine.Tetris _tetrisGame;
    private const double _updateInterval = 0.5;

    private int cellSize = 20;
    private int columns = 10;
    private int rows = 20;
    private int gap = 20;

    private KeyboardState _previousKeyboardState;
    private SpriteFont _font;
    private SignalRClient _signalRClient;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        _tetrisGame = new Engine.Tetris(rows, columns);
        _timeSinceLastUpdate = 0;
    }

    protected override void Initialize()
    {
        // Calculer la taille nécessaire pour contenir la grille
        int windowWidth = columns * cellSize * 2 + gap; // Largeur de deux grilles + espace entre les deux
        int windowHeight = rows * cellSize;

        // Définir la taille de la fenêtre
        _graphics.PreferredBackBufferWidth = windowWidth;
        _graphics.PreferredBackBufferHeight = windowHeight;
        _graphics.ApplyChanges(); // Appliquer les modifications

        _screenManager = new ScreenManager(GraphicsDevice, _spriteBatch);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _font = Content.Load<SpriteFont>("fonts/DefaultFont");
        _signalRClient = new SignalRClient("http://localhost:5198/game");
        _signalRClient.ConnectAsync().Wait();
        _cellTexture = new Texture2D(GraphicsDevice, 1, 1);
        _cellTexture.SetData(new[] {Color.White});
        _soundEffect = Content.Load<SoundEffect>("tetris_song");

        _soundEffectInstance = _soundEffect.CreateInstance();
        _soundEffectInstance.IsLooped = true;

        // _soundEffectInstance.Play();

        var firstScreen = new LobbyScreen(_screenManager, _spriteBatch, _font, _signalRClient);
        _screenManager.ChangeScreen(firstScreen);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _screenManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Nettoyer l'écran
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _screenManager.Draw(gameTime, _spriteBatch);

        base.Draw(gameTime);
    }
}