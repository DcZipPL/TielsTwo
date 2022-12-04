namespace Avalonia.Tiels.Pages.Create;

public class NotesPage : TilePage, ITileCreationPage
{
	public TileType CreationType
	{
		get => TileType.Tile; //TileType.Note;
	}
}