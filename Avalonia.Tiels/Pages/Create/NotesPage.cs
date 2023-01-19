namespace Avalonia.Tiels.Pages.Create;

// TODO: This abstraction in redundant. Refactor /Pages/Create/*Page.cs files to single one or something like this.
public class NotesPage : TilePage, ITileCreationPage
{
	public TileType CreationType
	{
		get => TileType.Tile; //TileType.Note;
	}
}