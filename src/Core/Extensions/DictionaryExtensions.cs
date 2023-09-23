namespace DKrOSS.Core.Extensions;

public static class DictionaryExtensions
{
    public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dict, (TKey key, TValue value) tuple)
    {
        dict.Add(tuple.key, tuple.value);
    }
}
