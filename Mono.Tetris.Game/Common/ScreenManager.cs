using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono.Tetris.Game.Common;
public class ScreenManager
{
    private IScreen _currentScreen;

    public ScreenManager(IScreen startScreen)
    {
        _currentScreen = startScreen;
        _currentScreen.Activate();
    }

    public void ChangeScreen(IScreen newScreen)
    {
        _currentScreen.Deactivate();
        _currentScreen = newScreen;
        _currentScreen.Activate();
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
