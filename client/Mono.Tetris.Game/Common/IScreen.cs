using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono.Tetris.Game.Common;

public interface IScreen
{
    // Mettre à jour la logique de l'écran
    void Update(GameTime gameTime);

    // Dessiner l'écran
    void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    // Méthode optionnelle pour gérer l'activation d'un écran
    void Activate();

    // Méthode optionnelle pour gérer la désactivation d'un écran
    void Deactivate();
}
