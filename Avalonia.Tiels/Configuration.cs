using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
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
    #region Loaders
    private Configuration(IControlledApplicationLifetime closer)
    {
    }

    public static Configuration Init(IControlledApplicationLifetime closer)
    {
        try
        {
            // Create config directory
            Directory.CreateDirectory(GetConfigDirectory());

            // Create config file
            var defaultConf = Path.Combine(Environment.CurrentDirectory, "global.default.toml");
            if (!File.Exists(defaultConf))
            {
                throw new FileNotFoundException("Couldn't find default configuration file.", defaultConf);
            }

            var defaultModel = File.ReadAllText(defaultConf);
            var model = Toml.ToModel<Models.GlobalModel>(defaultModel);
            if (model.Settings != null)
            {
                model.Settings.TilesPath = GetDefaultTilesDirectory();
                var toml = Toml.FromModel(model);

                File.WriteAllText(Path.Combine(GetConfigDirectory(), "global.toml"), toml);
            }
            else throw new NoNullAllowedException("The [settings] table is missing. Default configuration is possibly corrupted. Redownload file to continue.");
        }
        catch (UnauthorizedAccessException uae)
        {
            MessageWindow.Open("Error: " + uae.Message, "~0x0001 " + uae).Closed += (sender, args) =>
            {
                closer.Shutdown();
            };
            // TODO: Open as administrator / sudo
        }
        catch (Exception e)
        {
            MessageWindow.Open("Error: " + e.Message, "~0x0002 " + e).Closed += (sender, args) =>
            {
                closer.Shutdown();
            };
        }

        return new Configuration(closer);
    }

    public static Configuration Load(IControlledApplicationLifetime closer)
    {
        return new Configuration(closer);
    }
    #endregion

    #region Helpers

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

    #endregion

    #region Request Appearance
    public FluentThemeMode ReqGlobalTheme()
    {
        Enum.TryParse(ReqModel().Appearance!.Theme, true, out FluentThemeMode theme);
        return theme;
    }
    
    public WindowTransparencyLevel ReqGlobalTransparencyLevel()
    {
        return (WindowTransparencyLevel) ReqModel().Appearance!.Transparency;
    }
    
    public Color ReqGlobalColor()
    {
        return Util.ColorFromHex(ReqModel().Appearance!.Color);
    }
    #endregion

    #region Request Settings

    public string ReqTilesPath()
    {
        return ReqModel().Settings!.TilesPath ?? "";
    }
    
    public bool ReqAutostart()
    {
        return ReqModel().Settings!.AutoStart;
    }

    public bool ReqAutostartHideSettings()
    {
        return ReqModel().Settings!.HideSettings;
    }
    
    public bool ReqSpecialEffects()
    {
        return ReqModel().Settings!.SpecialEffects;
    }
    
    public bool ReqExperimental()
    {
        return ReqModel().Settings!.Experimental;
    }

    #endregion
    
    private Models.GlobalModel ReqModel()
    {
        var defaultModel = File.ReadAllText(Path.Combine(GetConfigDirectory(), "global.toml"));
        var model = Toml.ToModel<Models.GlobalModel>(defaultModel);
        if (model.Appearance == null || model.Settings == null)
            throw new NoNullAllowedException("The [settings] or [appearance] table is missing. Configuration is possibly corrupted.");
        return model;
    }
    
    #region Models
    public enum BarAlignment
    {
        Top,
        Bottom
    }
    
    public class Models
    {
        public class Appearance : ITomlMetadataProvider
        {
            public string? Theme { get; set; } // FluentThemeMode (lowercase)
            public string? Color { get; set; }
            public int Transparency { get; set; } // WindowTransparencyLevel
            
            TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }
        }
    
        public class Settings : ITomlMetadataProvider
        {
            public string? TilesPath { get; set; }
            public bool AutoStart { get; set; }
            public bool HideSettings { get; set; }
            public bool SpecialEffects { get; set; }
            public bool Experimental { get; set; }
            
            TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }
        }
    
        public class GlobalModel : ITomlMetadataProvider
        {
            public Settings? Settings { get; set; }
            public Appearance? Appearance { get; set; }
        
            TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }
        }

        public class TileModel : ITomlMetadataProvider
        {
            public string ID;
            public string Name;
            public bool Hidden;
            public Vector2 Size;
            public Vector2 Location;
            public int EditBarAlignment; // BarAlignment
            public Appearance? Appearance;
        
            public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
        }
    }
    #endregion
}