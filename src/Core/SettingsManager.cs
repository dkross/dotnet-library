using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DKrOSS.Core;

public abstract class SettingsManager<TSettings> where TSettings : ISettings<TSettings>
{
    private const UnixFileMode UnixFileMode700 =
        UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute;

    public const string SectionName = nameof(Settings);

    private readonly UTF8Encoding _utf8WithoutBomEncoding = new(false);
    private readonly string _relSettingsDirectoryPath;

    protected SettingsManager(string @namespace, string settingsFileName)
    {
        ArgumentNullException.ThrowIfNull(@namespace);
        ArgumentNullException.ThrowIfNull(settingsFileName);

        string[] namespaceElements = @namespace.Split('.');

        if (namespaceElements.Length == 0)
            throw new ArgumentException("Invalid namespace", nameof(@namespace));

        _relSettingsDirectoryPath = Path.Combine(namespaceElements);

        SettingsFileName = settingsFileName;
    }

    public virtual TSettings? Settings { get; set; }

    [JsonIgnore]
    public virtual string SettingsDirectoryPath => GetSettingsDirectoryPath();

    [JsonIgnore]
    public virtual string SettingsFileName { get; }

    [JsonIgnore]
    public virtual string SettingsFilePath => Path.Join(SettingsDirectoryPath, SettingsFileName);

    private void CreateSettingsDirectory()
    {
        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
#pragma warning disable CA1416
            Directory.CreateDirectory(SettingsDirectoryPath, UnixFileMode700);
#pragma warning restore CA1416
            return;
        }

        Directory.CreateDirectory(SettingsDirectoryPath);
    }

    public void Save()
    {
        if (!Directory.Exists(SettingsDirectoryPath))
            CreateSettingsDirectory();

        File.WriteAllText(SettingsFilePath, Serialize(), _utf8WithoutBomEncoding);
    }

    public void SaveDefaults()
    {
        TSettings? currentSettings = Settings;
        Settings = TSettings.Default;
        Save();
        Settings = currentSettings;
    }

    public async Task SaveAsync()
    {
        if (!Directory.Exists(SettingsDirectoryPath))
            CreateSettingsDirectory();

        await File.WriteAllTextAsync(SettingsFilePath, Serialize(), _utf8WithoutBomEncoding);
    }

    public async Task SaveDefaultsAsync()
    {
        TSettings? currentSettings = Settings;
        Settings = TSettings.Default;
        await SaveAsync();
        Settings = currentSettings;
    }

    private string GetSettingsDirectoryPath()
    {


        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
            {
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Join(localAppDataPath, _relSettingsDirectoryPath);
            }
            case PlatformID.Unix:
            {
                string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Join(homePath, ".config", _relSettingsDirectoryPath);
            }
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
            case PlatformID.Xbox:
            case PlatformID.MacOSX:
            case PlatformID.Other:
            default:
                throw new NotSupportedException(
                    $"Platform {Environment.OSVersion.Platform} is currently not supported.");
        }
    }

    private string Serialize()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(this, options).ReplaceLineEndings("\n");
    }
}
