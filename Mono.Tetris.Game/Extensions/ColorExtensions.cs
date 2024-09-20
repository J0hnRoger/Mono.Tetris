using Microsoft.Xna.Framework;

namespace Mono.Tetris.Game.Extensions;

public static class ColorExtensions
{
    public static Color? GetMonoGameColorByName(this Tetris.Engine.Color color)
    {
        return color.Name.ToLower() switch
        {
            "red" => Color.Red,
            "green" => Color.Green,
            "blue" => Color.Blue,
            "yellow" => Color.Yellow,
            "cyan" => Color.Cyan,
            "purple" => Color.Purple,
            "orange" => Color.Orange,
            _ => null
        };
    }
}