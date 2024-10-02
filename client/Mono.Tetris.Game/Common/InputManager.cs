using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono.Tetris.Engine;

namespace Mono.Tetris.Game.Common;

public class InputManager
{
    public event Action<string>? OnMoveExecuted;

    private double _timeSinceLastMove = 0;
    private bool _isHoldingKey = false;

    private const double InitialDelay = 0.2; // Délai initial en secondes
    private const double RepeatRate = 0.1; // Fréquence de répétition en secondes
    private KeyboardState _previousKeyboardState;
    private Keys? _lastKeyPressed = null;

    public TetrisAction? GetInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        _timeSinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;

        if (!_isHoldingKey)
        {
            _lastKeyPressed = null;
            _timeSinceLastMove = 0;
        }

        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            return HandleKeyRepeat(Keys.Up, "rotate", new RotateAction(), gameTime);
        }

        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            return HandleKeyRepeat(Keys.Left, "left", new MoveAction(-1, 0), gameTime);
        }

        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            return HandleKeyRepeat(Keys.Right, "right", new MoveAction(1, 0), gameTime);
        }

        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            return HandleKeyRepeat(Keys.Down, "down", new MoveAction(0, -1), gameTime);
        }

        if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
        {
            return HandleKeyRepeat(Keys.Space, "drop", new DropAction(), gameTime);
        }

        _previousKeyboardState = keyboardState;

        return null;
    }

    private TetrisAction? HandleKeyRepeat(Keys key, string moveName, TetrisAction action, GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(key))
        {
            // Si c'est la première fois qu'on appuie sur cette touche
            if (_lastKeyPressed != key)
            {
                _lastKeyPressed = key;
                _isHoldingKey = true;
                _timeSinceLastMove = 0;
                OnMoveExecuted?.Invoke(moveName);
                return action;
            }

            // Gérer le délai initial et la répétition
            if (_isHoldingKey && _timeSinceLastMove >= (InitialDelay + RepeatRate))
            {
                _timeSinceLastMove = 0;
                OnMoveExecuted?.Invoke(moveName);
                return action;
            }
        }

        return null;
    }
}