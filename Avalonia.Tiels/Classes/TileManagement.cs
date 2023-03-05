using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
		var i = 0;
		foreach (var systemEntry in Directory.EnumerateFileSystemEntries(configuration.Tiles[window.ID].Path))
		{
			// TODO: Get thumbnails from os
			var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
			var asset = assets.Open(new Uri("avares://Avalonia.Tiels/Assets/unknown.png"));

			Dispatcher.UIThread.Post(() =>
			{
				var thumbnail = new Image();//Util.SetSvgImage("/Assets/Icons/out/alert-octagon.svg", new Image());
				thumbnail.Source = new Bitmap(asset);
				window.entries.Add(new TileWindow.TileEntry(systemEntry, thumbnail));
			});

			// TODO: Better threading if possible
			/*Dispatcher.UIThread.Post(() =>
			{
				if (i >= window.GetCellAmount().Item1)
					window.EntryContent.RowDefinitions.Add(new RowDefinition(TileWindow.CELL_HEIGHT, GridUnitType.Pixel));
				
				// TODO: Get thumbnails from os
				var thumbnail = new Image();//Util.SetSvgImage("/Assets/Icons/out/alert-octagon.svg", new Image());
				var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
				var asset = assets.Open(new Uri("avares://Avalonia.Tiels/Assets/unknown.png"));
				thumbnail.Source = new Bitmap(asset);
				
				var entry = new EntryComponent
				{
					EntryName = Path.GetFileName(systemEntry),
					Preview = thumbnail
				};
				Grid.SetColumn(entry, window.GetCell(i).Item1);
				Grid.SetRow(entry, window.GetCell(i).Item2);
				window.EntryContent.Children.Add(entry);
				i++;
			});*/
		}
		
		Dispatcher.UIThread.Post(() =>
		{
			window.ReorderEntries();
		});
	}
}