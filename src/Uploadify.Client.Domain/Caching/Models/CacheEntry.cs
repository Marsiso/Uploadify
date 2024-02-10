namespace Uploadify.Client.Domain.Caching.Models;

public abstract class BaseCacheEntry
{
    protected BaseCacheEntry(DateTime lastChecked)
    {
        LastChecked = lastChecked;
    }

    public DateTime LastChecked { get; set; }
}

public class CacheEntry<TEntry> : BaseCacheEntry where TEntry : class
{
    public CacheEntry(TEntry? entry, DateTime lastChecked) : base(lastChecked)
    {
        Entry = entry;
    }

    public TEntry? Entry { get; set; }
}
