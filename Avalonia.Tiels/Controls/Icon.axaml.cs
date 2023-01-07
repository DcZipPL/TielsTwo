using System;
using Avalonia.Controls;
using Avalonia.Svg;

namespace Avalonia.Tiels.Controls;

public partial class Icon : UserControl
{
    public string Path { get; set; }
    public Icon()
    {
        InitializeComponent();
    }

    private void LoadIconOnInitialize(object? sender, EventArgs e)
    {
        Util.SetSvgImage(Path, IconImage);
    }
}