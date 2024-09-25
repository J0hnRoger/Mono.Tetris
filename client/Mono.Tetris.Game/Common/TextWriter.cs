using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono.Tetris.Game.Common;

public class TextWriter
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;

    public TextWriter(SpriteBatch spriteBatch, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _font = font;
    }

    public void DrawText(string text, Vector2 position, Color color)
    {
        _spriteBatch.DrawString(_font, text, position, color);
    }

    public void DrawCenteredText(string text, Vector2 centerPosition, Color color)
    {
        // Mesurer la taille du texte pour le centrer
        var textSize = _font.MeasureString(text);
        var position = new Vector2(centerPosition.X - textSize.X / 2, centerPosition.Y - textSize.Y / 2);
        _spriteBatch.DrawString(_font, text, position, color);
    }
}
