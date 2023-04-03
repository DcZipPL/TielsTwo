using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Image = System.Drawing.Image;

namespace Avalonia.Tiels.Controls;

public partial class EntryComponent : UserControl
{
	public IImage? Preview { get; set; }
	public string EntryName { get; set; } = "";
	public string Path { get; set; } = "";
	public FluentThemeMode Theme { get; set; }
	
	public EntryComponent()
	{
		InitializeComponent();
	}
	
	private void EntryClicked(object? sender, RoutedEventArgs e)
	{
		if (OperatingSystem.IsWindows())
		{
			using Process fireStarter = new Process();
			fireStarter.StartInfo.FileName = "explorer";
			fireStarter.StartInfo.Arguments = "\"" + Path + "\"";
			fireStarter.Start();
		} else if (OperatingSystem.IsLinux())
		{
			// TODO: Perhaps there is better way to do it in other OSes
			using Process fireStarter = new Process();
			fireStarter.StartInfo.FileName = "xdg-open";
			fireStarter.StartInfo.Arguments = "\"" + Path + "\"";
			fireStarter.Start();
		}
	}

	private void TextBlockInitialized(object? sender, EventArgs e)
	{
		FileNameTextBlock.Foreground = new SolidColorBrush(Theme == FluentThemeMode.Dark ? Colors.White : Colors.Black);
	}
}