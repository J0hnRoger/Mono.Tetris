﻿using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Tetris.Game.Common;
using Mono.Tetris.Game.Components;
using Mono.Tetris.Game.Infrastructure;

namespace Mono.Tetris.Game.Screens;

public class LobbyScreen : IScreen
{
    private InputText _inputText;

    private bool _isWaitingForOpponent = false;
    private SpriteFont _font;
    private KeyboardState _previousKeyboardState;
    private readonly TextWriter _textWriter;

    private bool _gameStarted = false; // Booléen pour savoir si le match a commencé
    private ScreenManager _screenManager;
    private SignalRClient _signalRClient; // Gestion de la communication serveur
    private readonly GameState _gameState;
    private bool _isInitialized;

    public LobbyScreen(ScreenManager screenManager, SpriteFont font,
        SignalRClient signalRClient, GameState gameState)
    {
        _screenManager = screenManager;
        _font = font;
        _signalRClient = signalRClient;
        _gameState = gameState;
        _inputText = new InputText(20);
    }

    public void Update(GameTime gameTime)
    {
        if (_gameStarted)
            return;

        // Mettre à jour la saisie du texte via le composant InputText
        _inputText.Update();

        // Si le joueur appuie sur Entrée, lancer le matchmaking
        if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_isWaitingForOpponent && _inputText.Text.Length > 0)
        {
            StartMatchmaking();
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        // Afficher le champ de texte pour le nom du joueur
        spriteBatch.DrawString(_font, "Entrez votre nom:", new Vector2(100, 100), Color.White);

        _inputText.Draw(spriteBatch, _font, new Vector2(100, 150), Color.Yellow);

        // Si en attente, afficher un message
        if (_isWaitingForOpponent)
        {
            spriteBatch.DrawString(_font, "En attente d'un adversaire...", new Vector2(100, 200), Color.Gray);
        }
        else
        {
            spriteBatch.DrawString(_font, "appuyez sur Entree pour lancer la partie", new Vector2(100, 250),
                Color.White);
        }

        spriteBatch.End();
    }

    public void Activate()
    {
        /* Logique d'activation */
    }

    public void Deactivate()
    {
        /* Logique de désactivation */
    }

    // Méthode pour lancer le matchmaking et attendre l'adversaire
    private void StartMatchmaking()
    {
        _isWaitingForOpponent = true;

        // Envoyer le nom du joueur au serveur pour débuter le matchmaking
        _signalRClient.SendPlayerName(_inputText.Text);
        _gameState.PlayerName = _inputText.Text;

        // Écouter le signal du serveur pour savoir quand le match peut démarrer
        _signalRClient.OnStartGame((opponentName) =>
        {
            _gameStarted = true;
            _gameState.OpponentName = opponentName;
            _screenManager.ChangeScreen("GameScreen");
        });
    }
}