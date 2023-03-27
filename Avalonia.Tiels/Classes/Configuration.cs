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

                } else { ErrorHandler.ShowErrorWindow(new InvalidDataException("Name of file: " + filePath + "has invalid Guid."), 0x0004); }
            }
            if (Path.GetExtension(filePath) == ".bin") // thumbnail
            {
                if (Guid.TryParse(Path.GetFileNameWithoutExtension(filePath), out var guid))
                {
                    if (!Tiles.ContainsKey(guid))
                        Tiles.Add(guid, new Tile());
                    
                    Tiles[guid]._thumbnailDbPath = filePath;
                } else { ErrorHandler.ShowErrorWindow(new InvalidDataException("Name of file: \"" + filePath + "\" has invalid Guid."), 0x0005); }
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
                throw new FileNotFoundException(App.I18n.GetString("DefaultConfigMissingError"), defaultConf);
            }
            
            // Check if Tile config exist
            var tileConf = Path.Combine(Environment.CurrentDirectory, "Defaults/tile.default.toml");
            if (!File.Exists(defaultConf))
            {
                throw new FileNotFoundException(App.I18n.GetString("DefaultConfigMissingError"), tileConf);
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
            throw ErrorHandler.ShowErrorWindow(uae, 0x0001);
            // TODO: Open as administrator / sudo
        }
        catch (Exception e)
        {
            throw ErrorHandler.ShowErrorWindow(e, 0x0002);
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
        return !Directory.Exists(GetConfigDirectory()) || !File.Exists(Path.Combine(GetConfigDirectory(), "global.toml"));
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
                ErrorHandler.Warn("GlobalTheme", "Couldn't load Appearance settings!");
                return FluentThemeMode.Light;
            }
            else
            {
                var result = Enum.TryParse(appearanceReq.Theme, true, out FluentThemeMode theme);
                if (result == false)
                {
                    ErrorHandler.Warn("GlobalTheme", "Couldn't parse theme from config!");
                }
                return theme;
            }
        }
        set
        {
            var model = ReqModel();
            model.Appearance.Theme = value.ToString().ToLower();
            SeedModel(model);
        }
    }

    public WindowTransparencyLevel GlobalTransparencyLevel
    {
        get
        {
            var appearanceReq = ReqModel().Appearance;
            if (appearanceReq == null) ErrorHandler.Warn("GlobalTransparencyLevel", "Couldn't load Appearance settings!");
            return appearanceReq != null ? (WindowTransparencyLevel)appearanceReq.Transparency : WindowTransparencyLevel.None;
        }
        set
        {
            var model = ReqModel();
            model.Appearance.Transparency = (int)value;
            SeedModel(model);
        }
    }

    public Color GlobalColor
    {
        get
        {
            var appearanceReq = ReqModel().Appearance;
            if (appearanceReq == null) ErrorHandler.Warn("GlobalColor", "Couldn't load Appearance settings!");
            return appearanceReq != null ? Color.Parse(appearanceReq.Color) : Color.FromRgb(255,0,255);
        }
        set
        {
            var model = ReqModel();
            model.Appearance.Color = Util.ColorToHex(value);
            SeedModel(model);
        }
    }
    #endregion

    #region Request Settings

    // TODO: Add more null checks

    [ConfigEntry] private float __Snapping;
    [ConfigEntry] private float __HandleHeight;

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
    
    [ConfigEntry] private bool __Autostart;
    [ConfigEntry] private bool __AutostartHideSettings;
    [ConfigEntry] private bool __Experimental;
    [ConfigEntry] private bool __ThumbnailsSettingsEnabled;
    [ConfigEntry] private bool __HideTileButtons;

    #endregion
    
    #region Tile Management
    public sealed class Tile
    {
        // ReSharper disable InconsistentNaming
        internal string _thumbnailDbPath;
        internal string _configPath;
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
        /// <returns>Guid of new Tile.</returns>
        /// <exception cref="Exception">Throws InvalidDataException if default configuration file don't have Size table.</exception>
        public static Guid CreateTileConfig(Configuration configAccess, string name, string path, double sizeX, double sizeY, bool overrideTheme, FluentThemeMode theme, WindowTransparencyLevel transparencyLevel, Color color)
        {
            var id = Guid.NewGuid();
            
            // TODO: Do bin
            var tileConf = System.IO.Path.Combine(Environment.CurrentDirectory, "Defaults/tile.default.toml");
            
            var defaultModel = File.ReadAllText(tileConf);
            var model = Toml.ToModel<Models.TileModel>(defaultModel);

            if (model.Size == null)
                throw ErrorHandler.ShowErrorWindow(new InvalidDataException("Tile config don't contain Size or it is null!") , 0x0006);
            
            if (model.Appearance == null)
                throw ErrorHandler.ShowErrorWindow(new InvalidDataException("Tile config don't Appearance size or it is null!") , 0x0007);

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

        public void SaveThumbnail(string path, string thumbnailPath)
        {
            int header = System.Text.Encoding.ASCII.GetByteCount("BPT");
            const uint maxBufferSize = sizeof(uint);
            const uint bufferSize = sizeof(uint) * maxBufferSize;
        
            byte[] bytes = File.ReadAllBytes(path);
        
            //File.WriteAllBytes(path, new []{});
        }
    
        public static void LoadThumbnails(string path)
        {
            int header = System.Text.Encoding.ASCII.GetByteCount("BPT");
            const uint maxBufferSize = sizeof(uint);
            const uint bufferSize = sizeof(uint) * maxBufferSize;
        
            byte[] bytes = File.ReadAllBytes(path);
            for (var i = (uint)(header + maxBufferSize + bufferSize); i <= bytes.Length; i++)
            {
            
            }
            //uint buffer = 2048;
        }
        
        #endregion

        #region Request
        
        public string Id
        {
            get { return ReqModel().Id ?? ""; }
            set
            {
                var model = ReqModel();
                model.Id = value;
                SeedModel(model);
            }
        }
        
        public string Name
        {
            get { return ReqModel().Name ?? ""; }
            set
            {
                var model = ReqModel();
                model.Name = value;
                SeedModel(model);
            }
        }
        
        public string Path
        {
            get { return ReqModel().Path ?? ""; }
            set
            {
                var model = ReqModel();
                model.Path = value;
                SeedModel(model);
            }
        }
        
        public bool Hidden
        {
            get { return ReqModel().Hidden; }
            set
            {
                var model = ReqModel();
                model.Hidden = value;
                SeedModel(model);
            }
        }
        
        public BarAlignment EditBarAlignment
        {
            get { return (BarAlignment)ReqModel().EditBarAlignment; }
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
            get
            {
                // TODO: Test this
                var appearanceReq = ReqModel().Appearance;
                return appearanceReq != null && (bool)appearanceReq.Override;
            }
            set
            {
                var model = ReqModel();
                model.Appearance.Override = value;
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
                var appearanceReq = ReqModel().Appearance;
                if (appearanceReq == null)
                {
                    ErrorHandler.Warn("GlobalTheme", "Couldn't load Appearance settings!");
                    return FluentThemeMode.Light;
                }
                else
                {
                    var result = Enum.TryParse(appearanceReq.Theme, true, out FluentThemeMode theme);
                    if (result == false)
                    {
                        ErrorHandler.Warn("GlobalTheme", "Couldn't parse theme from config!");
                    }
                    return theme;
                }
            }
            set
            {
                var model = ReqModel();
                model.Appearance.Theme = value.ToString().ToLower();
                SeedModel(model);
            }
        }

        public WindowTransparencyLevel TransparencyLevel
        {
            get
            {
                var appearanceReq = ReqModel().Appearance;
                if (appearanceReq == null) ErrorHandler.Warn("GlobalTransparencyLevel", "Couldn't load Appearance settings!");
                return appearanceReq != null ? (WindowTransparencyLevel)appearanceReq.Transparency : WindowTransparencyLevel.None;
            }
            set
            {
                var model = ReqModel();
                model.Appearance.Transparency = (int)value;
                SeedModel(model);
            }
        }

        public Color Color
        {
            get
            {
                var appearanceReq = ReqModel().Appearance;
                if (appearanceReq == null) ErrorHandler.Warn("GlobalColor", "Couldn't load Appearance settings!");
                return appearanceReq != null ? Color.Parse(appearanceReq.Color) : Color.FromRgb(255,0,255);
            }
            set
            {
                var model = ReqModel();
                model.Appearance.Color = Util.ColorToHex(value);
                SeedModel(model);
            }
        }
        #endregion
        
        #region Model Management
        private Models.TileModel ReqModel()
        {
            lock (_tileConfigLock)
            {
                var defaultModel = File.ReadAllText(_configPath);
                var model = Toml.ToModel<Models.TileModel>(defaultModel);
                if (model.Appearance == null || model.Size == null || model.Location == null)
                    throw new NoNullAllowedException(App.I18n.GetString("MissingSettingsAppearanceTableError"));
                return model;
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
            // TODO: Add try catch
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
            public bool Override { get; set; }
            public string? Theme { get; set; } // FluentThemeMode (lowercase)
            public string? Color { get; set; }
            public int Transparency { get; set; } // WindowTransparencyLevel
            
            TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }
        }
    
        public class Settings : ITomlMetadataProvider
        {
            public string? TilesPath { get; set; }
            public bool Autostart { get; set; }
            public bool AutostartHideSettings { get; set; }
            public bool ThumbnailsSettingsEnabled { get; set; }
            public bool Experimental { get; set; }
            public bool HideTileButtons { get; set; }
            public float Snapping { get; set; } // Snap to grid
            public float HandleHeight { get; set; }
            
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
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? Path { get; set; }
            public bool Hidden { get; set; }
            public int EditBarAlignment { get; set; } // BarAlignment
            public Vec2? Size { get; set; }
            public Vec2? Location { get; set; }
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
public class ConfigEntryAttribute : Attribute {}