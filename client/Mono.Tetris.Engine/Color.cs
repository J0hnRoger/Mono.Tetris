namespace Mono.Tetris.Engine;

public record Color
{
    public static List<string> Colors = new List<string>()
    {
        "Grey",
        "Red",
        "Green",
        "Blue",
        "Yellow",
        "Cyan",
        "Purple",
        "Orange"
    };
    
    public string Name { get; private set; }

    // Constructeur qui prend un nom de couleur ou un code hexadécimal
    public Color(string value)
    {
        if (!IsValidColorName(value))
            throw new ArgumentException("Invalid color name or hex value.");
        Name = value;
    }

    private bool IsValidColorName(string value)
    {
        return Colors.Contains(value);
    }
}