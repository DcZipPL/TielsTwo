using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Classes.Platform;
using Image = System.Drawing.Image;

namespace Avalonia.Tiels.Controls;

public partial class EntryComponent : UserControl
{
	public IImage? Preview { get; set; }
	public string EntryName { get; set; } = "";

	public string EntryNameShort
	{
		get
		{
			// TODO: Use fallback font
			Font font = new Font("Courier New", 14.0F);
			Image fakeImage = new Bitmap(1, 1);
			Graphics graphics = Graphics.FromImage(fakeImage);
			SizeF size = graphics.MeasureString(EntryName, font);

			if (size.Width < 240)
				return EntryName;

			var shortName = EntryName.Substring(0, EntryName.Length - 3);
			while (size.Width >= 240)
			{
				shortName = shortName.Substring(0, shortName.Length - 1);
				size = graphics.MeasureString(shortName, font);
			}
			return shortName + "...";
		}
	}

	public string Path { get; set; } = "";
	public FluentThemeMode Theme { get; set; }
	public FileAttribute Attribute { get; set; }
	
	public EntryComponent()
	{
		InitializeComponent();
		FileRenameBox.Text = Name;
	}
	
	private void EntryClicked(object? sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = "\"" + Path + "\"",
			UseShellExecute = true,
			Verb = "open"
		});
	}

	private void TextBlockInitialized(object? sender, EventArgs e)
	{
		FileNameTextBlock.Foreground = new SolidColorBrush(Theme == FluentThemeMode.Dark ? Colors.White : Colors.Black);
	}

	private void AttributeIconInitialized(object? sender, EventArgs e)
	{
		FileAttributeIcon.Text = Attribute switch
		{
			FileAttribute.Link => Icons.ExternalLink,
			FileAttribute.SymbolicLink => Icons.FolderSymlink,
			_ => ""
		};
	}

	private void RenameEntry(object? sender, RoutedEventArgs e)
	{
		FileRenameBox.IsVisible = true;
		FileRenameBox.Text = Name;
		FileRenameBox.Focus();
	}

	private void DeleteEntry(object? sender, RoutedEventArgs e)
	{
		try
		{
			FileSystem.SendFileToTrash(Path);
		}
		catch (Exception exception)
		{
			LoggingHandler.Error(exception, "EntryComponent");
		}
	}

	private void ShowInExplorer(object? sender, RoutedEventArgs e)
	{
		Process.Start("explorer.exe", $"/select, \"{Path}\"");
	}

	private void ApplyRename(object? sender, RoutedEventArgs e)
	{
		if (!FileRenameBox.IsVisible)
			return;
		
		FileRenameBox.IsVisible = false;
		EntryName = FileRenameBox.Text;
		
		// Check if directory or file.
		if (File.GetAttributes(Path).HasFlag(FileAttributes.Directory))
			Directory.Move(Path, Path.Remove(Path.LastIndexOf('\\')) + "\\" + EntryName);
		else
			File.Move(Path, Path.Remove(Path.LastIndexOf('\\')) + "\\" + EntryName);
	}
}