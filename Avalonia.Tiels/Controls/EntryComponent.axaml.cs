using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Classes.Platform;
using Bitmap = System.Drawing.Bitmap;
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
	public ThemeMode Theme { get; set; }
	public FileAttribute Attribute { get; set; }
	
	public EntryComponent()
	{
		InitializeComponent();

		// TODO: Add per tile combo box to change between Pixel Mode and Smooth Mode
		RenderOptions.SetBitmapInterpolationMode(EntryPreview, BitmapInterpolationMode.None);

		// This is temporary. I don't see easy way to implement Windows Explorer context menu in Avalonia C#.
		// If someone knows how to do it, please let me know. If you want to help, make a pull request.
		var appChooser = ContextMenuBuilder.CreateItem("Choose another app", Icons.None);
		appChooser.Click += (sender, args) => FileSystem.SpawnOpenWithDialog(Path);
		
		var openWithItems = new[] { appChooser };
		
		SelfButton.ContextMenu = new ContextMenuBuilder()
			.AddItem("Open", Icons.None, OpenEntry)
			.AddItemWithSubmenu("Open with...", Icons.MoreHorizontal, openWithItems)
			.AddSeparator()
			.AddItem("Rename", Icons.EditAlt, RenameEntry)
			.AddItem("Delete", Icons.Trash, DeleteEntry)
			.AddSeparator()
			.AddItem("Show in Explorer", Icons.FolderOpen, ShowInExplorer)
			.Build();
	}

	private void OpenEntry()
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = "\"" + Path + "\"",
			UseShellExecute = true,
			Verb = "open"
		});
	}

	private void RenameEntry()
	{
		FileNameTextBlock.IsVisible = false;
		FileAttributeIcon.IsVisible = false;
		
		FileRenameBox.IsVisible = true;
		FileRenameBox.Text = Name;
		FileRenameBox.Focus();
	}

	private void DeleteEntry()
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

	private void ShowInExplorer()
	{
		Process.Start("explorer.exe", $"/select, \"{Path}\"");
	}

	private void ApplyRename()
	{
		Debug.WriteLine("Lost focus: " + FileRenameBox.Text + " with " + EntryName);

		if (!FileRenameBox.IsVisible)
			return;
		
		if (FileRenameBox.Text == EntryName)
			return;
		
		if (String.IsNullOrEmpty(FileRenameBox.Text))
		{
			FileRenameBox.Text = EntryName;
			return;
		}
		
		FileNameTextBlock.IsVisible = true;
		FileAttributeIcon.IsVisible = true;

		FileRenameBox.IsVisible = false;
		EntryName = FileRenameBox.Text;
		FileNameTextBlock.Text = FileRenameBox.Text;
		
		// Check if directory or file.
		if (File.GetAttributes(Path).HasFlag(FileAttributes.Directory))
			Directory.Move(Path, Path.Remove(Path.LastIndexOf('\\')) + "\\" + EntryName);
		else
			File.Move(Path, Path.Remove(Path.LastIndexOf('\\')) + "\\" + EntryName);
		
		Path = Path.Remove(Path.LastIndexOf('\\')) + "\\" + EntryName;
	}
	
	private void FileRenameBoxInitialized(object? sender, EventArgs e) => FileRenameBox.Text = Name;
	private void TextBlockInitialized(object? sender, EventArgs e) => FileNameTextBlock.Foreground = new SolidColorBrush(Theme == ThemeMode.Dark ? Colors.White : Colors.Black);
	private void AttributeIconInitialized(object? sender, EventArgs e) => FileAttributeIcon.Text = Attribute switch
	{
		FileAttribute.Link => Icons.ExternalLink,
		FileAttribute.SymbolicLink => Icons.FolderSymlink,
		_ => ""
	};
	
	private void EntryClicked(object? sender, RoutedEventArgs e) => OpenEntry();
	private void EntryRenameLostFocus(object? sender, RoutedEventArgs e) => ApplyRename();
	private void FileRenameBoxKeyDown(object? sender, KeyEventArgs e) {
		if (e.Key == Key.Enter)
			ApplyRename();
	}
}