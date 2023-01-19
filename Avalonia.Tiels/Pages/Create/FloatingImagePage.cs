namespace Avalonia.Tiels.Pages.Create;

// TODO: This abstraction in redundant. Refactor /Pages/Create/*Page.cs files to single one or something like this.
public class FloatingImagePage : TilePage, ITileCreationPage
{
	public TileType CreationType
	{
		get => TileType.Tile; //TileType.FloatingImage;
	}
}