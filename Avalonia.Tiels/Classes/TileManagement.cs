using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
	public static void CreateTile(string name, string path, double width, double height, bool overriteTheme, ThemeMode? theme, TransparencyLevel? transparencyLevel, Color? color)
	{
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		var window = new TileWindow(
			Configuration.Tile.CreateTileConfig(App.Instance.Config, name, path, width, height, overriteTheme, theme ?? ThemeMode.Dark, transparencyLevel ?? TransparencyLevel.Transparent, color ?? Util.TILE_DARK_COLOR)
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
				// if file is .qoi file, skip it
				if (Path.GetExtension(systemEntry) == ".qoi")
				{
					var decoder = new QOIDecoder();
					var bytes = File.ReadAllBytes(systemEntry);
					var success = decoder.Decode(encoded: bytes, encodedSize: bytes.Length);
					if (success)
					{
						int width = decoder.GetWidth();
						int height = decoder.GetHeight();
						WriteableBitmap bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888);
						
						using (var bitmapLock = bitmap.Lock())
						{
							IntPtr buffer = bitmapLock.Address;
							int stride = bitmapLock.RowBytes;
							
							int[] pixels = decoder.GetPixels();
							int pixelCount = width * height;
							for (int i = 0; i < pixelCount; i++)
							{
								// Extract ARGB components from the integer value
								int pixel = pixels[i];
								byte alpha = (byte)((pixel >> 24) & 0xFF);
								byte red = (byte)((pixel >> 16) & 0xFF);
								byte green = (byte)((pixel >> 8) & 0xFF);
								byte blue = (byte)(pixel & 0xFF);

								// Write the pixel values to the bitmap buffer
								Marshal.WriteByte(buffer, i * 4 + 0, blue);
								Marshal.WriteByte(buffer, i * 4 + 1, green);
								Marshal.WriteByte(buffer, i * 4 + 2, red);
								Marshal.WriteByte(buffer, i * 4 + 3, alpha);
							}
						}

						window.entries.Add(new TileWindow.TileEntry(systemEntry, bitmap));
					}
				}
				else
				{
					var thumbnail = ThumbnailCsi.GetThumbnailImage(systemEntry, ThumbnailSize.Jumbo);
					window.entries.Add(new TileWindow.TileEntry(systemEntry, thumbnail));
				}
			}

			// TODO: Better threading if possible
			Dispatcher.UIThread.Post(() => { window.LoadEntries(0); });
		} catch (Exception e)
		{
			LoggingHandler.Error(e, $"Failed to load Tile content for Tile with ID: {window.ID}.");
		}
	}

	public static void DeleteTile(Guid id)
	{
		Configuration.Tile.DeleteTileConfig(App.Instance.Config, id);
		
		// Close Tile if it's open
		App.Instance.ActiveTileWindows.FirstOrDefault(x => x.ID == id)?.Close();
		
		Log.Debug($"Deleted Tile with ID: {id}.");
	}
}