namespace IndependentWork7;

public class Cache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _cache = new();
    private readonly Queue<TKey> _keysOrder = new();

    public Cache(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _capacity = capacity;
    }

    public int Count => _cache.Count;

    public void Add(TKey key, TValue value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            return;
        }

        if (_cache.Count == _capacity)
        {
            var oldestKey = _keysOrder.Dequeue();
            _cache.Remove(oldestKey);
        }

        _cache.Add(key, value);
        _keysOrder.Enqueue(key);
    }

    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException($"The key '{key}' was not found in the cache.");
    }

    public bool ContainsKey(TKey key) => _cache.ContainsKey(key);
}
