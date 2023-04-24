using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.Tiels.Classes.Platform;
using Serilog;

namespace Avalonia.Tiels.Classes;

public class TileManagement
{

	/// <summary>
	/// Creates new Tile with parameters.
	/// </summary>
	/// <param name="name">Name of Tile.</param>
	/// <param name="path">Path to Tile content.</param>
	/// <param name="width">Width of Tile.</param>
	/// <param name="height">Height of Tile.</param>
	public static void CreateTile(string name, string path, double width, double height, bool overriteTheme, FluentThemeMode? theme, WindowTransparencyLevel? transparencyLevel, Color? color)
	{
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		var window = new TileWindow(
			Configuration.Tile.CreateTileConfig(App.Instance.Config, name, path, width, height, overriteTheme, theme ?? FluentThemeMode.Dark, transparencyLevel ?? WindowTransparencyLevel.Transparent, color ?? Util.TILE_DARK_COLOR)
		);
		window.Show();
		App.Instance.ActiveTileWindows.Add(window);
		Log.Debug($"Created new Tile with name {name} and path {path}.");
	}

	/// <summary>
	/// Loads Tile from config with corresponding Guid.
	/// </summary>
	/// <param name="id">Guid of Tile.</param>
	public static TileWindow LoadTile(Guid id)
	{
		Log.Debug($"Loading Tile with ID: {id}.");
		var window = new TileWindow(id);
		window.Show();
		return window;
	}
	
	/// <summary>
	/// Loads Tile directories and files and shows them on UI thead.
	/// </summary>
	/// <param name="window">Tile window.</param>
	/// <param name="configuration">Configuration Access.</param>
	public static void LoadTileContent(TileWindow window, Configuration configuration)
	{
		// TODO: If icon matches with existing icon. Reuse it.
		foreach (var systemEntry in Directory.EnumerateFileSystemEntries(configuration.Tiles[window.ID].Path))
		{
			var thumbnail = ThumbnailCsi.GetThumbnailImage(systemEntry, ThumbnailSize.Jumbo);
			window.entries.Add(new TileWindow.TileEntry(systemEntry, thumbnail));
		}

		// TODO: Better threading if possible
		Dispatcher.UIThread.Post(() =>
		{
			window.LoadEntries(0);
		});
	}
}