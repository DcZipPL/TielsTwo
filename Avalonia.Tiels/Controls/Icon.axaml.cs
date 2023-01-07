using System;
using Avalonia.Controls;
using Avalonia.Svg;

namespace Avalonia.Tiels.Controls;

public partial class Icon : UserControl
{
    private string _path;
    public string Path
    {
        get => _path;
        set
        {
            _path = value;
            if (this.IsInitialized && IconImage.IsInitialized)
            {
                Util.SetSvgImage(value, IconImage);
            }
        }
    }

    public Icon()
    {
        InitializeComponent();
    }

    private void LoadIconOnInitialize(object? sender, EventArgs e)
    {
        Util.SetSvgImage(Path, IconImage);
    }
}