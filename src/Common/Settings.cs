using System.Text;
using DKrOSS.Core;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace DKrOSS.Common;

public abstract class Settings
{
    private const UnixFileMode UnixFileMode700 =
        UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute;

    private readonly UTF8Encoding _utf8EncodingWithoutBom = new(false);

    public virtual void Save(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(filePath));

        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        using FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var sw = new StreamWriter(fs, _utf8EncodingWithoutBom);
        sw.Write(json.ReplaceLineEndings("\n"));
    }

    public virtual async Task SaveAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(filePath));

        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await using FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var sw = new StreamWriter(fs, _utf8EncodingWithoutBom);
        await sw.WriteAsync(json.ReplaceLineEndings("\n"));
    }

    [Pure]
    protected static T? DeserializeObject<T>(string json) where T : Settings
    {
        return JsonConvert.DeserializeObject<T?>(json);
    }

    [Pure]
    protected static T? Load<T>(string filePath) where T : Settings
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(filePath));

        using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        string json = sr.ReadToEnd();
        return DeserializeObject<T>(json);
    }

    [Pure]
    protected static async Task<T?> LoadAsync<T>(string filePath) where T : Settings
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(filePath));

        await using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        string json = await sr.ReadToEndAsync();
        return DeserializeObject<T>(json);
    }

    [Pure]
    protected static string GetDefaultSettingsDirectoryPath(string @namespace)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(@namespace));

        string[] namespaceElements = @namespace.Split('.');

        if (namespaceElements.Length == 0)
            throw new ArgumentException(Resources.InvalidNamespace, nameof(@namespace));

        string relativePathFromNamespace = Path.Combine(namespaceElements);
        string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        return Path.Join(localAppDataPath, relativePathFromNamespace);
    }

    [Pure]
    protected static string GetDefaultSettingsFilePath(string @namespace, string settingsFileName)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(@namespace));

        if (string.IsNullOrWhiteSpace(settingsFileName))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(settingsFileName));

        return Path.Combine(GetDefaultSettingsDirectoryPath(@namespace), settingsFileName);
    }

    [Pure]
    protected static string? FindNearestExistingSettingsFilePath(
        string @namespace,
        string settingsFileName,
        FileAccess requestedAccess = FileAccess.Read)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(@namespace));

        if (string.IsNullOrWhiteSpace(settingsFileName))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(settingsFileName));


        string? nearestFilePath = FileUtils.FindNearestExistingFilePath(settingsFileName, requestedAccess);

        if (nearestFilePath is not null)
            return nearestFilePath;

        string defaultSettingsFilePath = GetDefaultSettingsFilePath(@namespace, settingsFileName);

        try
        {
            using FileStream fs = File.Open(defaultSettingsFilePath, FileMode.Open, requestedAccess,
                FileShare.None);
            return defaultSettingsFilePath;
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected static void CreateSettingsDirectory(string directoryPath)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Unix:
            case PlatformID.MacOSX:
#pragma warning disable CA1416
                Directory.CreateDirectory(directoryPath, UnixFileMode700);
#pragma warning restore CA1416
                break;
            default:
                Directory.CreateDirectory(directoryPath);
                break;
        }
    }
}
