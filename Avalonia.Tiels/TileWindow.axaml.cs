using System;
using System.Numerics;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels;

public partial class TileWindow : Window
{
	private App _app = (App)Application.Current!;
	
	public Guid ID { get; set; }
	
	public TileWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
	}

	private void OnLoad(object? sender, EventArgs e)
	{
		var loadTileThread = new Thread(() => this.LoadTile(_app.Config, ID));
	}

	public void LoadTile(Configuration configuration, Guid id)
	{
		if (configuration.TileExist(id))
		{
			
		}
	}
}