using System;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Tiels.Classes.Image;
using Avalonia.Tiels.Classes.Platform;
using Avalonia.Tiels.Classes.Platform.Helpers;
using Avalonia.Tiels.Classes.Style;
using Serilog;
using SkiaSharp;

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
	public static void CreateTile(string name, string path, double width, double height, bool overriteTheme, ThemeMode? theme, TransparencyLevel? transparencyLevel, Color? color)
	{
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		var window = new TileWindow(
			Configuration.Tile.CreateTileConfig(App.Instance.Config, name, path, width, height, overriteTheme, theme ?? ThemeMode.Dark, transparencyLevel ?? TransparencyLevel.Transparent, color ?? Palette.TILE_DARK_COLOR)
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
		try
		{
			foreach (var systemEntry in Directory.EnumerateFileSystemEntries(configuration.Tiles[window.ID].Path))
			{
				FileAttributes attr = File.GetAttributes(systemEntry);
				if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
				{
					// Icons by header
					byte[] head = new byte[8];
					using (BinaryReader reader = new BinaryReader(new FileStream(systemEntry, FileMode.Open)))
						reader.Read(head, 0, 8);

					string extension = Path.GetExtension(systemEntry);
					if (FileHeaders.IsPngImage(head)
					    || (FileHeaders.IsJpegImage(head) && (extension == ".jpg" || extension == ".jpeg" || extension == ".jpe")))
						CreateWithImage(window, systemEntry); // TODO: Support JXL
					else if (FileHeaders.IsQoiImage(head))
						CreateWithQoi(window, systemEntry);
					else
						CreateWithThumbnail(window, systemEntry);
				}
				else
				{
					// TODO: Custom directory icons
					CreateWithThumbnail(window, systemEntry);
				}
			}

			// TODO: Better threading if possible
			Dispatcher.UIThread.Post(() => { window.LoadEntries(0); });
		} catch (Exception e)
		{
			LoggingHandler.Error(e, $"Failed to load Tile content for Tile with ID: {window.ID}.");
		}
	}

	private static void CreateWithImage(TileWindow window, string systemEntry)
	{
		byte[] imageBytes = File.ReadAllBytes(systemEntry);
		using var stream = new MemoryStream(imageBytes);
		var skiaBitmap = SKBitmap.Decode(stream);
							
		window.entries.Add(new TileWindow.TileEntry(systemEntry, skiaBitmap.ConvertToAvaloniaBitmap()));
	}

	private static void CreateWithQoi(TileWindow window, string systemEntry)
	{
		var bytes = File.ReadAllBytes(systemEntry);
		var result = QoiImage.DecodeFromBytes(bytes);
		var thumbnail = result switch
		{
			{ IsSuccessful: true } => result.Value,
			{ IsSuccessful: false } => new SKBitmap(new SKImageInfo(1, 1)).ConvertToAvaloniaBitmap() // TODO: Use default icon
		};
		window.entries.Add(new TileWindow.TileEntry(systemEntry, thumbnail));
	}

	private static void CreateWithThumbnail(TileWindow window, string systemEntry)
	{
		var result = ThumbnailCsi.GetThumbnailImage(systemEntry, ThumbnailSize.ExtraLarge,
			ThumbnailSize.Large, ThumbnailSize.Small);
		var thumbnail = result switch
		{
			{ IsSuccessful: true } => result.Value,
			{ IsSuccessful: false } => ThumbnailCsi.GetShellIcon()
		};
		window.entries.Add(new TileWindow.TileEntry(systemEntry, thumbnail));
	}

	public static void DeleteTile(Guid id)
	{
		Configuration.Tile.DeleteTileConfig(App.Instance.Config, id);
		
		// Close Tile if it's open
		App.Instance.ActiveTileWindows.FirstOrDefault(x => x.ID == id)?.Close();
		
		Log.Debug($"Deleted Tile with ID: {id}.");
	}
}