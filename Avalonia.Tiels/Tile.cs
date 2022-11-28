using System.Numerics;

namespace Avalonia.Tiels;

public interface Tile
{
    public string TypeName { get; }
    
    public string Name { get; set; }
    public Vector2 Size { get; set; }
}