using System.Security.Cryptography;
using DKrOSS.Core;
using JetBrains.Annotations;

namespace DKrOSS.Common;

public static class FileUtilsAsync
{
    [Pure]
    public static async Task<byte[]> ComputeFileHashAsync(
        string path,
        HashAlgorithm hashAlgorithm,
        CancellationToken cancellationToken = default)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        if (hashAlgorithm is null)
            throw new ArgumentNullException(nameof(hashAlgorithm));

        if (!File.Exists(path))
            throw new FileNotFoundException(Resources.RequiredFileDoesNotExist, path);

        await using FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await hashAlgorithm.ComputeHashAsync(fs, cancellationToken);
    }

    [Pure]
    public static async Task<byte[]> ComputeFileHashAsync(string path, CancellationToken cancellationToken = default)
    {
        return await ComputeFileHashAsync(path, SHA256.Create(), cancellationToken);
    }
}
