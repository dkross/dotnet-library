using JetBrains.Annotations;

namespace DKrOSS.Core;

public static class FileUtils
{
    [Pure]
    public static string? FindNearestExistingFilePath(string fileName, FileAccess? requestedAccess = null)
    {
        if (fileName is null)
            throw new ArgumentNullException(nameof(fileName));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException(
                Resources.StringIsNullOrEmpty, nameof(fileName));

        string currentDirectory = Directory.GetCurrentDirectory();
        string[] pathElements = currentDirectory.Split(Path.DirectorySeparatorChar);
        string? foundSettingsFilePath = null;

        for (int i = pathElements.Length; i > 0; i--)
        {
            string searchDirectoryPath = Path.Combine(pathElements.Take(i).ToArray());
            string searchFilePath = Path.Combine(searchDirectoryPath, fileName);

            if (!File.Exists(searchFilePath))
                continue;

            if (requestedAccess is null)
            {
                foundSettingsFilePath = searchFilePath;
                break;
            }

            try
            {
                using FileStream fs = File.Open(searchFilePath, FileMode.Open, (FileAccess)requestedAccess,
                    FileShare.None);
                foundSettingsFilePath = searchFilePath;
                break;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return foundSettingsFilePath;
    }
}
