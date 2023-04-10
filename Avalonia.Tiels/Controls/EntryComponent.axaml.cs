using System;
using System.Diagnostics;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;
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
			
			if (size.Width >= 240)
			{
				return EntryName.Substring(0, 12) + "...";
			}

			return EntryName;
		}
	}

	public string Path { get; set; } = "";
	public FluentThemeMode Theme { get; set; }
	public FileAttribute Attribute { get; set; }
	
	public EntryComponent()
	{
		InitializeComponent();
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
}