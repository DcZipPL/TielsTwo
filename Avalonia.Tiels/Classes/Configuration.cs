using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Tomlyn;
using Tomlyn.Model;

namespace Avalonia.Tiels.Classes;

public partial class Configuration
{
    private readonly object _readWriteLock = new object();
    private readonly object _thumbnailLock = new object();

    public Dictionary<Guid, Tile> Tiles = new Dictionary<Guid, Tile>();

    #region Loaders
    private Configuration(IControlledApplicationLifetime closer)
    {
        // Load Tile configs
        if (!Directory.Exists(GetTilesConfigDirectory()))
            Directory.CreateDirectory(GetTilesConfigDirectory());
        foreach (var filePath in Directory.EnumerateFiles(GetTilesConfigDirectory()))
        {
            if (Path.GetExtension(filePath) == ".toml") // config
            {
                if (Guid.TryParse(Path.GetFileNameWithoutExtension(filePath), out var guid))
                {
                    if (!Tiles.ContainsKey(guid))
                        Tiles.Add(guid, new Tile());
                    
                    Tiles[guid]._configPath = filePath;

                } else { LoggingHandler.Error(new InvalidDataException("Name of file: " + filePath + "has invalid Guid."), nameof(Configuration) + "&0"); }
            }
            if (Path.GetExtension(filePath) == ".bin") // thumbnail
            {
                if (Guid.TryParse(Path.GetFileNameWithoutExtension(filePath), out var guid))
                {
                    if (!Tiles.ContainsKey(guid))
                        Tiles.Add(guid, new Tile());
                    
                    Tiles[guid]._thumbnailDbPath = filePath;
                } else { LoggingHandler.Error(new InvalidDataException("Name of file: \"" + filePath + "\" has invalid Guid."), nameof(Configuration) + "&1"); }
            }
        }
    }

    /// <summary>
    /// Initializes and checks configuration files.
    /// </summary>
    /// <returns>Returns initialized configuration.</returns>
    /// <exception cref="FileNotFoundException">If config don't exist throws.</exception>
    /// <exception cref="NoNullAllowedException">If in config Settings table don't exist then throws.</exception>
    /// <exception cref="Exception">Throws other exceptions underway if something happen.</exception>
    public static Configuration Init(IControlledApplicationLifetime closer)
    {
        try
        {
            // Create config directory
            Directory.CreateDirectory(GetConfigDirectory());

            // Create config file
            var defaultConf = Path.Combine(Environment.CurrentDirectory, "Defaults/global.default.toml");
            if (!File.Exists(defaultConf))
            {
                throw LoggingHandler.Error(new FileNotFoundException(App.I18n.GetString("DefaultConfigMissingError"), defaultConf), "Configuration::Init");
            }
            
            // Check if Tile config exist
            var tileConf = Path.Combine(Environment.CurrentDirectory, "Defaults/tile.default.toml");
            if (!File.Exists(defaultConf))
            {
                throw LoggingHandler.Error(new FileNotFoundException(App.I18n.GetString("DefaultConfigMissingError"), tileConf), "Configuration::Init");
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
            else throw LoggingHandler.Error(new NoNullAllowedException(App.I18n.GetString("MissingConfigError")), "Configuration::Init");
        }
        catch (UnauthorizedAccessException uae)
        {
            throw LoggingHandler.Error(uae, nameof(Configuration) + "&2");
            // TODO: Open as administrator / sudo
        }
        catch (Exception e)
        {
            throw LoggingHandler.Error(e, nameof(Configuration) + "&3");
        }

        return Load(closer);
    }

    /// <summary>
    /// Loads configuration.
    /// </summary>
    /// <returns>Returns loaded configuration.</returns>
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
    
    public static string GetTilesConfigDirectory(string withFile = "")
    {
        return Path.Combine(GetConfigDirectory(), "tiles/", withFile);
    }
    
    public static bool IsFirstStartup()
    {
        return !(Directory.Exists(GetConfigDirectory()) || File.Exists(Path.Combine(GetConfigDirectory(), "global.toml")));
    }

    #endregion

    #region Request Appearance
    public FluentThemeMode GlobalTheme
    {
        get
        {
            var appearanceReq = ReqModel().Appearance;
            if (appearanceReq == null)
            {
                LoggingHandler.Warn("GlobalTheme", "Couldn't load Appearance settings! Fallback to Light theme.");
                return FluentThemeMode.Light;
            }

            var result = Enum.TryParse(appearanceReq.Theme, true, out FluentThemeMode theme);
            if (!result)
                LoggingHandler.Warn("GlobalTheme", "Couldn't parse theme from config! Fallback to Light theme.");
            return theme;
        }
        set
        {
            var model = ReqModel();
            model.Appearance!.Theme = value.ToString().ToLower();
            SeedModel(model);
        }
    }

    public WindowTransparencyLevel GlobalTransparencyLevel
    {
        get
        {
            var appearanceReq = ReqModel().Appearance;
            if (appearanceReq == null) LoggingHandler.Warn("GlobalTransparencyLevel", "Couldn't load Appearance settings!");
            return appearanceReq != null ? (WindowTransparencyLevel)appearanceReq.Transparency : WindowTransparencyLevel.None;
        }
        set
        {
            var model = ReqModel();
            model.Appearance!.Transparency = (int)value;
            SeedModel(model);
        }
    }

    public Color GlobalColor
    {
        get
        {
            var appearanceReq = ReqModel().Appearance;
            if (appearanceReq == null) LoggingHandler.Warn("GlobalColor", "Couldn't load Appearance settings! Fallback to #FF00FF.");
            return appearanceReq != null ? Color.Parse(appearanceReq.Color) : Color.FromRgb(255,0,255);
        }
        set
        {
            var model = ReqModel();
            model.Appearance!.Color = Util.ColorToHex(value);
            SeedModel(model);
        }
    }
    #endregion

    #region Request Settings

    // TODO: Add more null checks

    // ReSharper disable InconsistentNaming
    #pragma warning disable CS0414
    #pragma warning disable CS0169
    
    [ConfigEntry(Group = "Settings")] private float __Snapping;
    [ConfigEntry(Group = "Settings")] private float __HandleHeight;
    [ConfigEntry(Group = "Settings")] private string __TilesPath = "";
    [ConfigEntry(Group = "Settings")] private string __Language = "";
    [ConfigEntry(Group = "Settings")] private bool __AutoStart;
    [ConfigEntry(Group = "Settings")] private bool __AutoStartHideSettings;
    [ConfigEntry(Group = "Settings")] private bool __Experimental;
    [ConfigEntry(Group = "Settings")] private bool __ThumbnailsSettingsEnabled;
    [ConfigEntry(Group = "Settings")] private uint __EntryLimit;
    
    #pragma warning restore CS0414
    #pragma warning restore CS0169
    // ReSharper restore InconsistentNaming

    #endregion
    
    #region Tile Management
    public sealed partial class Tile
    {
        // ReSharper disable InconsistentNaming
        internal string _thumbnailDbPath = "";
        internal string _configPath = "";
        private readonly object _tileConfigLock = new object();
        // ReSharper restore InconsistentNaming

        #region Helpers
        
        public bool TileExist()
        {
            if (!Directory.Exists(GetConfigDirectory())) return false;
        
            if (!Directory.Exists(GetTilesConfigDirectory()))
                Directory.CreateDirectory(GetTilesConfigDirectory());
            
            return File.Exists(_configPath);
        }

        /// <summary>
        /// Creates configuration files and caches for Tile.
        /// </summary>
        /// <param name="configAccess">Access to configuration.</param>
        /// <param name="name">Name of new Tile.</param>
        /// <param name="path">Path to the Tile content.</param>
        /// <param name="sizeX">Width of Tile.</param>
        /// <param name="sizeY">Height of Tile.</param>
        /// <param name="overrideTheme">Override global theme.</param>
        /// <param name="theme">Theme of Tile.</param>
        /// <param name="transparencyLevel">Transparency level of Tile.</param>
        /// <param name="color">Color of Tile.</param>
        /// <returns>Guid of new Tile.</returns>
        public static Guid CreateTileConfig(Configuration configAccess, string name, string path, double sizeX, double sizeY, bool overrideTheme, FluentThemeMode theme, WindowTransparencyLevel transparencyLevel, Color color)
        {
            var id = Guid.NewGuid();
            
            // TODO: Do bin
            
            // Load default config
            var model = new Models.TileModel();
            try
            {
                var defaultTileConfPath =
                    System.IO.Path.Combine(Environment.CurrentDirectory, "Defaults/tile.default.toml");
                var defaultModel = File.ReadAllText(defaultTileConfPath);
                model = Toml.ToModel<Models.TileModel>(defaultModel);
            }
            catch (Exception e)
            {
                LoggingHandler.Warn(nameof(CreateTileConfig), e.ToString());
            }

            // Check if default config is valid
            if (model.Size == null)
            {
                model.Size = new Configuration.Models.Vec2();
                LoggingHandler.Warn(nameof(CreateTileConfig), "Default Tile config don't contain Size or it is null!");
            }

            if (model.Appearance == null)
            {
                model.Appearance = new Configuration.Models.Appearance();
                LoggingHandler.Warn(nameof(CreateTileConfig), "Default Tile config don't Appearance size or it is null!");
            }

            // Assign values
            model.Id = id.ToString();
            model.Path = path;
            model.Name = name;
            model.Size.X = sizeX;
            model.Size.Y = sizeY;
            model.Appearance.Override = overrideTheme;
            model.Appearance.Color = Util.ColorToHex(color);
            model.Appearance.Theme = theme.ToString().ToLower();
            model.Appearance.Transparency = (int)transparencyLevel;
            
            Directory.CreateDirectory(GetDefaultTilesDirectory());
            var toml = Toml.FromModel(model);

            File.WriteAllText(GetTilesConfigDirectory(id + ".toml"), toml);
            File.WriteAllBytes(GetTilesConfigDirectory(id + ".bin"), new []{(byte)0b0000_0000_0000_0001});
            
            if (!configAccess.Tiles.ContainsKey(id))
                configAccess.Tiles.Add(id, new Tile());
            
            configAccess.Tiles[id]._configPath = GetTilesConfigDirectory(id + ".toml");
            configAccess.Tiles[id]._thumbnailDbPath = GetTilesConfigDirectory(id + ".bin");

            return id;
        }
        
        public static void DeleteTileConfig(Configuration configAccess, Guid id)
        {
            if (!configAccess.Tiles.ContainsKey(id)) return;
            
            if (File.Exists(configAccess.Tiles[id]._configPath))
                File.Delete(configAccess.Tiles[id]._configPath);
            
            if (File.Exists(configAccess.Tiles[id]._thumbnailDbPath))
                File.Delete(configAccess.Tiles[id]._thumbnailDbPath);
            
            configAccess.Tiles.Remove(id);
        }

        #endregion

        #region Request
        
        // ReSharper disable InconsistentNaming
        #pragma warning disable CS0414
        #pragma warning disable CS0169
        
        [ConfigEntry] private string __Id = "";
        [ConfigEntry] private string __Name = "";
        [ConfigEntry] private string __Path = "";
        [ConfigEntry] private bool __Hidden;
        [ConfigEntry] private bool __HideTileButtons;

        #pragma warning restore CS0414
        #pragma warning restore CS0169
        // ReSharper restore InconsistentNaming

        public BarAlignment EditBarAlignment
        {
            get => (BarAlignment)ReqModel().EditBarAlignment;
            set
            {
                var model = ReqModel();
                model.EditBarAlignment = (int)value;
                SeedModel(model);
            }
        }
        
        public Vector2 Location
        {
            get
            {
                var size = ReqModel().Location ?? new Models.Vec2();
                return new Vector2((float)size.X, (float)size.Y);
            }
            set
            {
                var model = ReqModel();
                model.Location = new Models.Vec2 {X = value.X, Y = value.Y};
                SeedModel(model);
            }
        }
        
        public Vector2 Size
        {
            get
            {
                var size = ReqModel().Size ?? new Models.Vec2();
                return new Vector2((float)size.X, (float)size.Y);
            }
            set
            {
                var model = ReqModel();
                model.Size = new Models.Vec2 {X = value.X, Y = value.Y};
                SeedModel(model);
            }
        }

        public bool IsOverriden
        {
            get => ReqModel().Appearance!.Override;
            set
            {
                var model = ReqModel();
                model.Appearance!.Override = value;
                SeedModel(model);
            }
        }
        
        #endregion
        
        // TODO: Duplicate from above
        #region Request Appearance
        public FluentThemeMode Theme
        {
            get
            {
                var result = Enum.TryParse(ReqModel().Appearance!.Theme, true, out FluentThemeMode theme);
                if (!result)
                    LoggingHandler.Error(new Exception($"Couldn't parse {nameof(theme)} from config!"), nameof(Configuration.Tile) + "->" + nameof(Theme));
                return result ? theme : FluentThemeMode.Light;
            }
            set
            {
                var model = ReqModel();
                model.Appearance!.Theme = Enum.GetName(value)!.ToLower();
                SeedModel(model);
            }
        }

        public WindowTransparencyLevel TransparencyLevel
        {
            get
            {
                var appearanceReq = ReqModel().Appearance;
                return appearanceReq != null ? (WindowTransparencyLevel)appearanceReq.Transparency : WindowTransparencyLevel.None;
            }
            set
            {
                var model = ReqModel();
                model.Appearance!.Transparency = (int)value;
                SeedModel(model);
            }
        }

        public Color Color
        {
            get
            {
                var result = Color.TryParse(ReqModel().Appearance!.Color, out var color);
                if (!result)
                    LoggingHandler.Warn($"{nameof(Configuration.Tile)}{{{ReqModel().Id}}} -> {nameof(Color)}", $"Couldn't parse {nameof(color)} from config! Using transparent color instead.");
                return result ? color : Color.FromArgb(0, 0, 0, 0);
            }
            set
            {
                var model = ReqModel();
                model.Appearance!.Color = Util.ColorToHex(value);
                SeedModel(model);
            }
        }
        #endregion
        
        #region Model Management
        private Models.TileModel ReqModel()
        {
            lock (_tileConfigLock)
            {
                try
                {
                    var plainModel = File.ReadAllText(_configPath);
                    var model = Toml.ToModel<Models.TileModel>(plainModel);
                    model.Appearance ??= new Models.Appearance();
                    model.Size ??= new Models.Vec2();
                    model.Location ??= new Models.Vec2();
                    return model;
                } catch (Exception e)
                {
                    LoggingHandler.Warn($"{nameof(Configuration.Tile)} -> {nameof(ReqModel)}", e.ToString());
                    return new Models.TileModel();
                }
            }
        }

        private void SeedModel(Models.TileModel model)
        {
            lock (_tileConfigLock)
            {
                var toml = Toml.FromModel(model);

                File.WriteAllText(_configPath, toml);
            }
        }
        #endregion
    }
    #endregion
    
    #region Model Management
    private Models.GlobalModel ReqModel()
    {
        lock (_readWriteLock)
        {
            try
            {
                var defaultModel = File.ReadAllText(Path.Combine(GetConfigDirectory(), "global.toml"));
                var model = Toml.ToModel<Models.GlobalModel>(defaultModel);
                model.Appearance ??= new Models.Appearance();
                model.Settings ??= new Models.Settings();
                return model;
            } catch (Exception e)
            {
                LoggingHandler.Warn($"{nameof(Configuration)} -> {nameof(ReqModel)}", e.ToString());
                return new Models.GlobalModel();
            }
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
    #endregion
    
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
            public bool Override { get; set; } = false;
            public string? Theme { get; set; } = "dark"; // FluentThemeMode (lowercase)
            public string? Color { get; set; } = Util.ColorToHex(Util.TILE_DARK_COLOR); // Color (hex)
            public int Transparency { get; set; } = 1; // WindowTransparencyLevel
            
            TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }
        }
    
        public class Settings : ITomlMetadataProvider
        {
            public string? TilesPath { get; set; }
            public string? Language { get; set; }
            public bool AutoStart { get; set; } = true;
            public bool AutoStartHideSettings { get; set; } = true;
            public bool ThumbnailsSettingsEnabled { get; set; } = false;
            public bool Experimental { get; set; } = false;
            public float Snapping { get; set; } = 5.0f; // Snap to grid
            public float HandleHeight { get; set; } = 28.0f; // Height of the handle
            public uint EntryLimit { get; set; } = 100; // Max number of entries in the history
            
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
            public string? Id { get; set; } = Guid.Empty.ToString();
            public string? Name { get; set; }
            public string? Path { get; set; }
            public bool Hidden { get; set; } = false;
            public bool HideTileButtons { get; set; } = false;
            public int EditBarAlignment { get; set; } = 0; // BarAlignment
            public Vec2? Size { get; set; } = new Vec2 { X = 200, Y = 100 };
            public Vec2? Location { get; set; } = new Vec2 { X = 100, Y = 100 };
            public Appearance? Appearance { get; set; }
            
            TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }
        }
        
        public record Vec2
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
    }
    #pragma warning restore CS8618
    #endregion
}

[AttributeUsage(AttributeTargets.Field)]
public class ConfigEntryAttribute : Attribute {
    public ConfigEntryAttribute()
    {
        Group = "";
    }

    public string Group { get; set; }
}