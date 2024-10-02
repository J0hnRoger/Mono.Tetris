using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    private readonly TextWriter _textWriter;
    private Texture2D _cellTexture;

    private int cellSize = 20;
    private int columns = 10;
    private int rows = 20;

    private readonly Engine.Tetris _tetrisGame;
    private readonly Engine.Tetris _opponentTetrisGame;

    private bool _allPlayersReady;
    private ITetrominoFactory _tetrominoFactory;
    private InputManager _inputManager;
    private NetworkInputManager _networkInputManager;

    public GameScreen(GameState gameState, GraphicsDevice graphicsDevice,
        TextWriter textWriter, SignalRClient signalRClient)
    {
        if (string.IsNullOrEmpty(gameState.OpponentName) || string.IsNullOrEmpty(gameState.PlayerName))
            throw new NullReferenceException("Names of the players should be filled");

        // Permet d'injecter par la suite la factory du server ou du client
        // en fonction du mode local ou online
        _tetrominoFactory = new TetrominoFactory();
        _signalRClient = signalRClient;

        _inputManager = new InputManager();
        _inputManager.OnMoveExecuted += move => {
            Task.Run(async () => await _signalRClient.SendMove(move.ToString().ToLower()));
        };
        
        // Manager pour relayer les mouvements de l'adversaire 
        _networkInputManager = new NetworkInputManager();

        _tetrisGame = new Engine.Tetris(rows, columns, gameState.PlayerName);
        _tetrisGame.OnTetrominoAdded += TetrominoAdded;

        _opponentTetrisGame = new Engine.Tetris(rows, columns, gameState.OpponentName);

        _textWriter = textWriter;
        _cellTexture = new Texture2D(graphicsDevice, 1, 1);
        _cellTexture.SetData(new[] { Color.White });

        _signalRClient.OnMoveReceived(OnOpponentMoveReceived);
        _signalRClient.OnTetrominoReceived(OnTetrominoReceived);

        _signalRClient.SendPlayerReady(gameState.PlayerName);
        _signalRClient.OnStartMatch(() => {
            _allPlayersReady = true;
        });
    }

    private void TetrominoAdded(Tetromino tetromino)
    {
        Debug.WriteLine(
            $"Player {_tetrisGame.PlayerName} receive SendAddTetromino: {tetromino} from {_opponentTetrisGame.PlayerName}");
        _ = _signalRClient.SendAddTetromino(_tetrisGame.PlayerName, tetromino);
    }

    private void OnTetrominoReceived(string playerName, TetrominoDto tetrominoAdded)
    {
        var addedTetromino = Tetromino.FromName(tetrominoAdded.Name);
        _opponentTetrisGame.AddTetromino(addedTetromino);
    }

    public void OnOpponentMoveReceived(string playerName, string move)
    {
        Debug.WriteLine($"Move received from {playerName}: {move}");
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
            case "down":
                _opponentTetrisGame.MoveTetromino(0, -1);
                break;
            case "drop":
                _opponentTetrisGame.DropTetromino();
                break;
            default:
                Debug.WriteLine($"Unknown move: {move}");
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        if (!_allPlayersReady)
            return;

        if (_tetrisGame.CurrentTetromino == null)
            _tetrisGame.AddTetromino(_tetrominoFactory.GetRandom());

        double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        _timeSinceLastUpdate += deltaTime;

        var localAction = _inputManager.GetInput(gameTime);
        if (localAction != null)
        {
            Debug.WriteLine($"Input pressed {localAction}");
            localAction.Execute(_tetrisGame);
        }

        var networkAction = _networkInputManager.GetNetworkInput();
        if (networkAction != null)
            networkAction.Execute(_opponentTetrisGame);

        if (_timeSinceLastUpdate >= _updateInterval)
        {
            _tetrisGame.Play();
            _opponentTetrisGame.Play();

            _timeSinceLastUpdate = 0;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        Vector2 playerGridPosition = new(0, 40);
        int tetrisGridSize = cellSize * columns;
        int gap = 20;
        Vector2 opponentGridPosition = new(tetrisGridSize + gap, 40);

        DrawGrid(_tetrisGame, spriteBatch, playerGridPosition);
        DrawGrid(_opponentTetrisGame, spriteBatch, opponentGridPosition);

        float player1TextX = playerGridPosition.X + tetrisGridSize / 2 - 50; // Centrer au-dessus de la grille 1
        float player2TextX = opponentGridPosition.X + tetrisGridSize / 2 - 50; // Centrer au-dessus de la grille 2
        float vsTextX = (player1TextX + player2TextX) / 2 - 20;

        _textWriter.DrawCenteredText(_tetrisGame.PlayerName,
            new Vector2(player1TextX, playerGridPosition.Y - 25), Color.White);
        _textWriter.DrawCenteredText(_opponentTetrisGame.PlayerName,
            new Vector2(player2TextX, playerGridPosition.Y - 25), Color.White);

        Vector2 vsPosition = new Vector2(vsTextX,
            playerGridPosition.Y - 25);

        _textWriter.DrawCenteredText("VS", vsPosition, Color.Yellow);

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