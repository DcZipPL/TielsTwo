using System;
using System.IO;
using System.Numerics;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Controls;

namespace Avalonia.Tiels;

public partial class TileWindow : Window
{
	public Guid ID { get; set; }
	
	public TileWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
		EditBar.Background = new SolidColorBrush((Color)EditBarColor());
	}

	public Color EditBarColor()
	{
		return Color.Parse(App.Instance.Config.GlobalTheme == FluentThemeMode.Dark ? "#25000000" : "#25ffffff");
	}

	private void OnLoad(object? sender, EventArgs e)
	{
		var loadConfig = new Thread(() => this.LoadConfigs(App.Instance.Config, ID));
		loadConfig.Start();
		
		var loadContentThread = new Thread(() => this.LoadContent(App.Instance.Config, ID));
		loadContentThread.Start();
	}

	public void LoadConfigs(Configuration configuration, Guid id)
	{
	}
	
	public void LoadContent(Configuration configuration, Guid id)
	{
		foreach (var systemEntry in Directory.EnumerateFileSystemEntries(configuration.Tiles[id].Path))
		{
			// TODO: Better threading if possible
			Dispatcher.UIThread.Post(() =>
			{
				var entry = new EntryComponent();
				entry.EntryName = Path.GetFileName(systemEntry);
				EntryContent.Children.Add(entry);
			});
		}
	}
}