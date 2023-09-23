namespace DKrOSS.Core;

public interface ISettings<out TSettings>
{
    public static abstract TSettings Default { get; }
    public bool Validate(bool throwOnFailure = false);
}
