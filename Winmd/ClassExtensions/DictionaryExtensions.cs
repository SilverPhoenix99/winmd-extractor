namespace Winmd.ClassExtensions;

internal static class DictionaryExtensions
{
    public static TValue ComputeIfMissing<TKey, TValue>(
        this IDictionary<TKey, TValue> self,
        TKey key,
        Func<TKey, TValue> compute
    )
    {
        if (self.TryGetValue(key, out var value))
        {
            return value;
        }

        lock (self)
        {
            if (self.TryGetValue(key, out value))
            {
                return value;
            }

            value = compute.Invoke(key);
            self.Add(key, value);
        }

        return value;
    }
}
