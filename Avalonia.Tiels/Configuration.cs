using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Security;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Themes.Fluent;
using Tomlyn;
using Tomlyn.Model;

namespace Avalonia.Tiels;

public class Configuration
{
    public Configuration(IControlledApplicationLifetime closer)
    {
        try
        {
            // Create config directory
            Directory.CreateDirectory(GetConfigDirectory());

            // Create config file
            string defaultConf = Path.Combine(Environment.CurrentDirectory, "global.default.toml");
            if (!File.Exists(defaultConf))
            {
                throw new FileNotFoundException("Couldn't find default configuration file.", defaultConf);
            }

            string defaultModel = File.ReadAllText(defaultConf);
            GlobalModel model = Toml.ToModel<GlobalModel>(defaultModel);
            model.Settings.TilesPath = GetDefaultTilesDirectory();
            string toml = Toml.FromModel(model);
            
            File.WriteAllText(toml, Path.Combine(GetConfigDirectory(), "global.toml"));
        }
        catch (UnauthorizedAccessException uae)
        {
            MessageWindow.Open("Error: "+uae.Message, "~0x0001 " + uae).Closed += (sender, args) =>
            {
                closer.Shutdown();
            };
            // Open as administrator / sudo
        }
        catch (Exception e)
        {
            MessageWindow.Open("Error: "+e.Message, "~0x0002 " + e).Closed += (sender, args) =>
            {
                closer.Shutdown();
            };
        }
    }

    public static Configuration Load()
    {
        return null;
    }

    private static string GetDefaultTilesDirectory()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Tiels");
    }
    
    public static string GetConfigDirectory()
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config/tiels");
        if (OperatingSystem.IsWindows())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tiels");
        
        return "./TielsConfig";
    }
    public static bool IsFirstStartup()
    {
        return !Directory.Exists(GetConfigDirectory());
    }
    
    // Models
    public class AppearanceModel : ITomlMetadataProvider
    {
        public string? Theme; // FluentThemeMode (lowercase)
        public string? Color;
        public int TransparencyLevel; // WindowTransparencyLevel
        
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
    }
    
    public class SettingsModel : ITomlMetadataProvider
    {
        public string? TilesPath;
        public bool AutoStart;
        public bool HideSettings;
        public bool SpecialEffects;
        public bool Experimental;
        
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
    }
    
    public class GlobalModel : ITomlMetadataProvider
    {
        public string? amount;
        public SettingsModel? Settings;
        public AppearanceModel? Appearance;
        
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
        public int EditBarAlignment; // BarAlignment
        public AppearanceModel? Appearance;
        
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
    }
}