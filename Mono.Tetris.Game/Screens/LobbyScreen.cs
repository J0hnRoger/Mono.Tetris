using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Tetris.Game.Common;

namespace Mono.Tetris.Game.Screens;

public class LobbyScreen : IScreen
{
    private readonly string playerName = "";

    // Méthodes de l'interface IScreen
    public void Update(GameTime gameTime)
    {
        // Logique pour saisir le nom du joueur
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        // spriteBatch.DrawString(""); // Dessiner le champ de texte
        spriteBatch.End();
    }

    public void Activate()
    {
        // Logique à exécuter lors de l'activation de l'écran
    }

    public void Deactivate()
    {
        // Logique à exécuter lors de la désactivation de l'écran
    }
}
