using System;
using Avalonia.Controls;

namespace Avalonia.Tiels;

public partial class MessageWindow : Window
{
	public string? WindowTitle { get; set; }
	public string? Message { get; set; }
	
	public MessageWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
	}

	public static MessageWindow Open(string? title, string? message)
	{
		MessageWindow msgw = new MessageWindow();
		msgw.WindowTitle = title;
		msgw.Message = message;
		msgw.Show();
		return msgw;
	}

	private void OnOpened(object? sender, EventArgs e)
	{
		if (WindowTitle != null && Message != null)
		{
			this.Title = WindowTitle;
			this.Width = Message.Length * 2.5f;
			MessageText.Text = Message;
		}
	}
}