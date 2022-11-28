using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Security;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Themes.Fluent;
using Tomlyn;
using Tomlyn.Model;

namespace Avalonia.Tiels;

public class Configuration
{
    public Configuration()
    {
        try
        {
            Directory.CreateDirectory(GetConfigDirectory());
            
        }
        catch (UnauthorizedAccessException uae)
        {
            MessageWindow.Open("Error: "+uae.Message, "~0x0001 " + uae);
            // Open as administrator / sudo
        }
        catch (Exception e)
        {
            MessageWindow.Open("Error: "+e.Message, "~0x0002 " + e);
        }
    }

    public static Configuration Load()
    {
        var gm = new GlobalModel
        {
            Settings =
            {
                TilesLocation = Path.Combine(GetTilesDirectory(), "Tiels"), AutoStart = true, HideSettings = true,
                Experimental = false, SpecialEffects = true
            },
            Appearance =
            {
                Color = Color.Black, TransparencyLevel = WindowTransparencyLevel.Transparent,
                Theme = FluentThemeMode.Dark
            }, PropertiesMetadata = new TomlPropertiesMetadata()
        };
        string toml = Toml.FromModel(gm);
        return null;
    }

    public static string GetTilesDirectory()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    }
    
    public static string GetConfigDirectory()
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
            return "~/.config/tiels";
        if (OperatingSystem.IsWindows())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tiels");
        
        return "./TielsConfig";
    }
    public static bool IsFirstStartup()
    {
        return Directory.Exists(GetConfigDirectory());
    }
    
    // Models
    public class AppearanceModel
    {
        public FluentThemeMode Theme;
        public Color Color;
        public WindowTransparencyLevel TransparencyLevel;
    }
    
    public class SettingsModel
    {
        public string TilesLocation;
        public bool AutoStart;
        public bool HideSettings;
        public bool SpecialEffects;
        public bool Experimental;
    }
    
    public class GlobalModel : ITomlMetadataProvider
    {
        public SettingsModel Settings;
        public AppearanceModel Appearance;

        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
    }
    
    public enum BarAlignment
    {
        Top,
        Bottom
    }
    
    public class TileModel : ITomlMetadataProvider
    {
        public string ID;
        public string Name;
        public bool Hidden;
        public Vector2 Size;
        public Vector2 Location;
        public BarAlignment EditBarAlignment;
        public AppearanceModel? Appearance;
        
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
    }
}