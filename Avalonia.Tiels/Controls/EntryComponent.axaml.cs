using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Classes.Platform;
using Avalonia.Tiels.Classes.Style;
using SkiaSharp;

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
			SKPaint font = new SKPaint
			{
				TextSize = 14.0f,
				Typeface = SKTypeface.FromFamilyName("Courier New")
			};

			float width = font.MeasureText(EntryName);

			if (width < 240)
				return EntryName;

			var shortName = EntryName.Substring(0, EntryName.Length - 3);
			while (width >= 240)
			{
				shortName = shortName.Substring(0, shortName.Length - 1);
				width = font.MeasureText(shortName);
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
		var appChooser = ContextMenuBuilder.CreateItem(App.I18n.GetString("ContextMenuChooseAnotherApp")!, Icons.None);
		appChooser.Click += (sender, args) => FileSystem.SpawnOpenWithDialog(Path);
		
		var openWithItems = new[] { appChooser };
		
		SelfButton.ContextMenu = new ContextMenuBuilder()
			.AddItem(App.I18n.GetString("ContextMenuOpen")!, Icons.None, OpenEntry)
			.AddItemWithSubmenu(App.I18n.GetString("ContextMenuOpenWith")!, Icons.MoreHorizontal, openWithItems)
			.AddSeparator()
			.AddItem(App.I18n.GetString("ContextMenuRename")!, Icons.EditAlt, RenameEntry)
			.AddItem(App.I18n.GetString("ContextMenuDelete")!, Icons.Trash, DeleteEntry)
			.AddSeparator()
			.AddItem(App.I18n.GetString("ContextMenuShowInExplorer")!, Icons.FolderOpen, ShowInExplorer)
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
		_ => " "
	};
	
	private void EntryClicked(object? sender, RoutedEventArgs e) => OpenEntry();
	private void EntryRenameLostFocus(object? sender, RoutedEventArgs e) => ApplyRename();
	private void FileRenameBoxKeyDown(object? sender, KeyEventArgs e) {
		if (e.Key == Key.Enter)
			ApplyRename();
	}
}