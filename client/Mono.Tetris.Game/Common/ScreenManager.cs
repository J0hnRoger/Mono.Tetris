using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono.Tetris.Game.Common;

public class ScreenManager
{
    private IScreen _currentScreen;
    private Dictionary<string, Func<IScreen>> _scenes = new();
    private GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private readonly GameState _gameState;

    public GraphicsDevice GetGraphicsDevice() => _graphicsDevice;
    public SpriteBatch GetSpriteBatch() => _spriteBatch;

    public ScreenManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameState gameState)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _gameState = gameState;
    }

    public void RegisterScene(string sceneName, Func<IScreen> screenFactory)
    {
        _scenes[sceneName] = screenFactory;
    }

    public void ChangeScreen(string sceneName)
    {
        if (_scenes.ContainsKey(sceneName))
        {
            
            _currentScreen?.Deactivate();
            _currentScreen = _scenes[sceneName]();
            _currentScreen.Activate();
        }
        else
        {
            throw new Exception($"Scene {sceneName} not found.");
        }
    }

    public void Update(GameTime gameTime)
    {
        _currentScreen.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _currentScreen.Draw(gameTime, spriteBatch);
    }
}