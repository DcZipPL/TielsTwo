using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Controls;

public class ContextMenuBuilder
{
	private List<Control> _items = new();

	public ContextMenuBuilder() {}
	
	public ContextMenuBuilder AddItem(string name, Icons icon, Action action)
	{
		var item = CreateItem(name, icon); 
		item.Click += (sender, args) => action();
		_items.Add(item);
		
		return this;
	}

	public ContextMenuBuilder AddItemWithSubmenu(string name, Icons icon, IEnumerable<Control> submenu)
	{
		var item = CreateItem(name, icon);
		item.Items = submenu;
		_items.Add(item);
		
		return this;
	}
	
	public ContextMenuBuilder AddSeparator()
	{
		_items.Add(new Separator());
		
		return this;
	}
	
	public ContextMenu Build()
	{
		return new ContextMenu { Items = _items };
	}

	public static MenuItem CreateItem(string name, Icons icon)
	{
		return new MenuItem {
			Header = name,
			VerticalAlignment = VerticalAlignment.Center,
			Icon = new TextBlock
			{
				Text = icon,
				FontSize = 16,
				FontFamily = Icons.FONT,
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			}
		};
	}
}