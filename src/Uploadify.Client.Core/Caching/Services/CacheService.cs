using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Caching.Models;
using static System.String;

namespace Uploadify.Client.Core.Caching.Services;

public class CacheService
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromHours(1);

    private readonly ApiCallWrapper _apiCallWrapper;
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<CacheService> _logger;

    public CacheService(ApiCallWrapper apiCallWrapper, ILocalStorageService localStorageService, ILogger<CacheService> logger)
    {
        _apiCallWrapper = apiCallWrapper;
        _localStorageService = localStorageService;
        _logger = logger;
    }

    public async Task<TEntry?> GetValues<TEntry>(
        string? cacheKey,
        Func<ApiCallWrapper, Task<TEntry?>> selector,
        bool useCache = true,
        TimeSpan? duration = default,
        CancellationToken cancellationToken = default)
        where TEntry : class
    {
        if (IsNullOrWhiteSpace(cacheKey))
        {
            return null;
        }

        _logger.LogInformation($"Service: '{nameof(CacheService)}' Action: 'GetValues<{typeof(TEntry).Name}>' Message: 'Retrieving cached values.'.");
        var cachedEntry = await _localStorageService.GetItemAsync<CacheEntry<TEntry>>(cacheKey, cancellationToken);
        if (cachedEntry != null)
        {
            duration ??= DefaultDuration;

            if (DateTime.UtcNow < cachedEntry.LastChecked + duration.Value)
            {
                _logger.LogInformation($"Service: '{nameof(CacheService)}' Action: 'GetValues<{typeof(TEntry).Name}>' Message: 'Returning cached values.'.");
                return cachedEntry.Entry;
            }
        }

        _logger.LogInformation($"Service: '{nameof(CacheService)}' Action: 'GetValues<{typeof(TEntry).Name}>' Message: 'Fetching values.'.");
        var entry = await selector(_apiCallWrapper);
        if (entry != null)
        {
            _logger.LogInformation($"Service: '{nameof(CacheService)}' Action: 'GetValues<{typeof(TEntry).Name}>' Message: 'Caching fetched values.'.");
            await _localStorageService.SetItemAsync(cacheKey, new CacheEntry<TEntry>(entry, DateTime.UtcNow), cancellationToken: cancellationToken);
        }

        _logger.LogInformation($"Service: '{nameof(CacheService)}' Action: 'GetValues<{typeof(TEntry).Name}>' Message: 'Returning fetched values.'.");
        return entry;
    }
}
