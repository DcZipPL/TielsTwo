using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels;

public partial class MessageWindow : Window
{
	public MessageWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
	}

	public static MessageWindow Open(string title, string message)
	{
		MessageWindow msgw = new MessageWindow();
		msgw.Title = title;
		msgw.MessageText.Text = message;
		msgw.Show();
		return msgw;
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}