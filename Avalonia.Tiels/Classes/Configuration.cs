using System;
using System.Buffers.Text;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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
    private readonly object _readWriteLock = new object();
    
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
                throw new FileNotFoundException(App.I18n.GetString("DefaultConfigMissingError"), defaultConf);
            }

            var defaultModel = File.ReadAllText(defaultConf);
            var model = Toml.ToModel<Models.GlobalModel>(defaultModel);
            if (model.Settings != null)
            {
                model.Settings.TilesPath = GetDefaultTilesDirectory();
                Directory.CreateDirectory(GetDefaultTilesDirectory());
                var toml = Toml.FromModel(model);

                File.WriteAllText(Path.Combine(GetConfigDirectory(), "global.toml"), toml);
            }
            else throw new NoNullAllowedException(App.I18n.GetString("MissingConfigError"));
        }
        catch (UnauthorizedAccessException uae)
        {
            ErrorHandler.ShowErrorWindow(uae, "~(0x0001)");
            throw;
            // TODO: Open as administrator / sudo
        }
        catch (Exception e)
        {
            ErrorHandler.ShowErrorWindow(e, "~(0x0002)");
            throw;
        }

        return new Configuration(closer);
    }

    public static Configuration Load(IControlledApplicationLifetime closer)
    {
        return new Configuration(closer);
    }
    #endregion

    #region Helpers

    public static string GetDefaultTilesDirectory()
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
        return !Directory.Exists(GetConfigDirectory()) || !File.Exists(Path.Combine(GetConfigDirectory(), "global.toml"));
    }

    #endregion

    #region Request Appearance
    public FluentThemeMode GlobalTheme
    {
        get { Enum.TryParse(ReqModel().Appearance!.Theme, true, out FluentThemeMode theme); return theme; }
        set
        {
            var model = ReqModel();
            model.Appearance!.Theme = value.ToString().ToLower();
            SeedModel(model);
        }
    }

    public WindowTransparencyLevel GlobalTransparencyLevel
    {
        get { return (WindowTransparencyLevel) ReqModel().Appearance!.Transparency; }
        set
        {
            var model = ReqModel();
            model.Appearance!.Transparency = (int)value;
            SeedModel(model);
        }
    }

    public Color GlobalColor
    {
        get { return Util.ColorFromHex(ReqModel().Appearance!.Color); }
        set
        {
            var model = ReqModel();
            model.Appearance!.Color = Util.ColorToHex(value);
            SeedModel(model);
        }
    }
    #endregion

    #region Request Settings

    public string TilesPath
    {
        get { return ReqModel().Settings!.TilesPath ?? ""; }
        set
        {
            var model = ReqModel();
            model.Settings!.TilesPath = value;
            SeedModel(model);
        }
    }
    
    public bool Autostart
    {
        get{ return ReqModel().Settings!.AutoStart; }
        set
        {
            var model = ReqModel();
            model.Settings!.AutoStart = value;
            SeedModel(model);
        }
    }

    public bool AutostartHideSettings
    {
        get{ return ReqModel().Settings!.HideSettings; }
        set
        {
            var model = ReqModel();
            model.Settings!.HideSettings = value;
            SeedModel(model);
        }
    }
    
    public bool SpecialEffects
    {
        get { return ReqModel().Settings!.SpecialEffects; }
        set
        {
            var model = ReqModel();
            model.Settings!.SpecialEffects = value;
            SeedModel(model);
        }
    }
    
    public bool Experimental
    {
        get { return ReqModel().Settings!.Experimental; }
        set
        {
            var model = ReqModel();
            model.Settings!.Experimental = value;
            SeedModel(model);
        }
    }

    #endregion
    
    #region Tile Management

    public bool TileExist(Guid id)
    {
        if (!Directory.Exists(GetConfigDirectory())) return false;
        
        var tilesConfig = Path.Combine(GetConfigDirectory(), "tiles/");
        if (!Directory.Exists(tilesConfig))
            Directory.CreateDirectory(tilesConfig);
        else
            return File.Exists(Path.Combine(tilesConfig, id.ToString(), ".toml"));

        return false;
    }

    #endregion
    
    private Models.GlobalModel ReqModel()
    {
        lock (_readWriteLock)
        {
            var defaultModel = File.ReadAllText(Path.Combine(GetConfigDirectory(), "global.toml"));
            var model = Toml.ToModel<Models.GlobalModel>(defaultModel);
            if (model.Appearance == null || model.Settings == null)
                throw new NoNullAllowedException(App.I18n.GetString("MissingSettingsAppearanceTableError"));
            return model;
        }
    }

    private void SeedModel(Models.GlobalModel model)
    {
        lock (_readWriteLock)
        {
            var toml = Toml.FromModel(model);

            File.WriteAllText(Path.Combine(GetConfigDirectory(), "global.toml"), toml);
        }
    }
    
    #region Models
    #pragma warning disable CS8618
    public enum BarAlignment
    {
        Top,
        Bottom
    }
    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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
            public string Id;
            public string Name;
            public bool Hidden;
            public Vector2 Size;
            public Vector2 Location;
            public int EditBarAlignment; // BarAlignment
            public Appearance? Appearance;
        
            public TomlPropertiesMetadata? PropertiesMetadata { get; set; }
        }
    }
    #pragma warning restore CS8618
    #endregion
}