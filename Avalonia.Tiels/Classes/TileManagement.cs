using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.Tiels.Controls;

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
		var window = new TileWindow(
			Configuration.Tile.CreateTileConfig(App.Instance.Config, name, path, width, height, overriteTheme, theme ?? FluentThemeMode.Dark, transparencyLevel ?? WindowTransparencyLevel.Transparent, color ?? Util.TILE_DARK_COLOR)
		);
		window.Show();
	}

	/// <summary>
	/// Loads Tile from config with corresponding Guid. 
	/// </summary>
	/// <param name="id">Guid of Tile.</param>
	public static void LoadTile(Guid id)
	{
		var window = new TileWindow(id);
		window.Show();
	}
	
	/// <summary>
	/// Loads Tile directories and files and shows them on UI thead. 
	/// </summary>
	/// <param name="window">Tile window.</param>
	/// <param name="configuration">Configuration Access.</param>
	public static void LoadTileContent(TileWindow window, Configuration configuration)
	{
		foreach (var systemEntry in Directory.EnumerateFileSystemEntries(configuration.Tiles[window.ID].Path))
		{
			// TODO: Better threading if possible
			Dispatcher.UIThread.Post(() =>
			{
				// TODO: Get thumbnails from os
				var thumbnail = Util.SetSvgImage("/Assets/Icons/out/alert-octagon.svg", new Image());
				var entry = new EntryComponent
				{
					EntryName = Path.GetFileName(systemEntry),
					Preview = thumbnail
				};
				window.EntryContent.Children.Add(entry);
			});
		}
	}
}