using System.Reflection;
using JetBrains.Annotations;

namespace DKrOSS.Core.Extensions;

public static class AssemblyExtensions
{
    [Pure]
    public static string? GetProductVersion(this Assembly assembly)
    {
        return assembly
            .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()
            ?.InformationalVersion;
    }
}
