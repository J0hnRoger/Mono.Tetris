using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono.Tetris.Game.Components;

public class InputText
{
    private string _text;
    private int _maxLength;
    private KeyboardState _previousKeyboardState;

    public string Text => _text; // Exposer le texte saisi

    public InputText(int maxLength)
    {
        _text = string.Empty;
        _maxLength = maxLength;
    }

    // Gérer la saisie du texte
    public void Update()
    {
        var keyboardState = Keyboard.GetState();

        foreach (var key in keyboardState.GetPressedKeys())
        {
            if (!_previousKeyboardState.IsKeyDown(key))
            {
                // Gérer la suppression avec Backspace
                if (key == Keys.Back && _text.Length > 0)
                {
                    _text = _text.Substring(0, _text.Length - 1);
                }
                // Ajouter des caractères si la longueur est inférieure au maximum
                else if (_text.Length < _maxLength)
                {
                    string newChar = KeyToString(key);
                    if (!string.IsNullOrEmpty(newChar))
                    {
                        _text += newChar;
                    }
                }
            }
        }

        _previousKeyboardState = keyboardState;
    }

    // Convertir une touche en chaîne de caractères
    private string KeyToString(Keys key)
    {
        // Gérer les lettres
        if (key >= Keys.A && key <= Keys.Z)
        {
            return key.ToString();
        }
        // Gérer les chiffres
        else if (key >= Keys.D0 && key <= Keys.D9)
        {
            return (key - Keys.D0).ToString();
        }
        return string.Empty;  // Retourner une chaîne vide si la touche n'est pas valide
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color)
    {
        // Affiche le texte saisi
        spriteBatch.DrawString(font, _text, position, color);
    }
}
