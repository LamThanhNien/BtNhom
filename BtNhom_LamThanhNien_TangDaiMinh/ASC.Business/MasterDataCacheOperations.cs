using System.Text.Json;
using ASC.Business.Interfaces;
using ASC.Business.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ASC.Business
{
    public class MasterDataCacheOperations : IMasterDataCacheOperations
    {
        private const string MasterDataCacheName = "MasterDataCache";
        private const string LocalMasterDataCacheName = "MasterDataCache:Local";
        private static readonly TimeSpan LocalCacheDuration = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan DistributedCacheTimeout = TimeSpan.FromMilliseconds(250);
        private static readonly SemaphoreSlim RefreshLock = new(1, 1);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = null
        };

        private readonly IDistributedCache _cache;
        private readonly IMasterDataOperations _masterData;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MasterDataCacheOperations> _logger;

        public MasterDataCacheOperations(
            IDistributedCache cache,
            IMasterDataOperations masterData,
            IMemoryCache memoryCache,
            ILogger<MasterDataCacheOperations> logger)
        {
            _cache = cache;
            _masterData = masterData;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task CreateMasterDataCacheAsync()
        {
            await RefreshMasterDataCacheAsync(forceRefresh: true);
        }

        public async Task<MasterDataCache> GetMasterDataCacheAsync()
        {
            if (_memoryCache.TryGetValue(LocalMasterDataCacheName, out MasterDataCache? localCache) && localCache is not null)
            {
                return localCache;
            }

            var distributedCache = await TryGetDistributedCacheAsync();
            if (distributedCache is not null)
            {
                SetLocalCache(distributedCache);
                return distributedCache;
            }

            return await RefreshMasterDataCacheAsync(forceRefresh: false);
        }

        private async Task<MasterDataCache> RefreshMasterDataCacheAsync(bool forceRefresh)
        {
            await RefreshLock.WaitAsync();
            try
            {
                if (!forceRefresh && _memoryCache.TryGetValue(LocalMasterDataCacheName, out MasterDataCache? cache) && cache is not null)
                {
                    return cache;
                }

                var masterDataCache = await BuildMasterDataCacheAsync();
                SetLocalCache(masterDataCache);
                await TrySetDistributedCacheAsync(masterDataCache);

                return masterDataCache;
            }
            finally
            {
                RefreshLock.Release();
            }
        }

        private async Task<MasterDataCache> BuildMasterDataCacheAsync()
        {
            var masterDataKeys = await _masterData.GetAllMasterKeysAsync();
            var masterDataValues = await _masterData.GetAllMasterValuesAsync();

            var activeKeys = masterDataKeys
                .Where(p => p.IsActive && !string.IsNullOrWhiteSpace(p.Key))
                .Select(p => new MasterDataKeyModel
                {
                    RowKey = p.Id,
                    PartitionKey = p.Key,
                    Name = p.Key,
                    IsActive = p.IsActive
                })
                .ToList();

            var keyLookup = activeKeys.ToDictionary(p => p.RowKey, p => p.PartitionKey, StringComparer.OrdinalIgnoreCase);

            var activeValues = masterDataValues
                .Where(p => p.IsActive)
                .Select(p => new MasterDataValueModel
                {
                    RowKey = p.Id,
                    PartitionKey = keyLookup.TryGetValue(p.MasterDataKeyId, out var key) ? key : string.Empty,
                    Name = p.Value,
                    IsActive = p.IsActive
                })
                .Where(p => !string.IsNullOrWhiteSpace(p.PartitionKey))
                .ToList();

            return new MasterDataCache
            {
                Keys = activeKeys,
                Values = activeValues
            };
        }

        private void SetLocalCache(MasterDataCache masterDataCache)
        {
            _memoryCache.Set(LocalMasterDataCacheName, masterDataCache, LocalCacheDuration);
        }

        private async Task<MasterDataCache?> TryGetDistributedCacheAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(DistributedCacheTimeout);
                var payload = await _cache.GetStringAsync(MasterDataCacheName, cts.Token);
                if (string.IsNullOrWhiteSpace(payload))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<MasterDataCache>(payload, JsonOptions);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Distributed cache read timed out for {CacheKey}.", MasterDataCacheName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache read failed for {CacheKey}. Falling back to local cache.", MasterDataCacheName);
                return null;
            }
        }

        private async Task TrySetDistributedCacheAsync(MasterDataCache masterDataCache)
        {
            try
            {
                using var cts = new CancellationTokenSource(DistributedCacheTimeout);
                var payload = JsonSerializer.Serialize(masterDataCache, JsonOptions);
                await _cache.SetStringAsync(MasterDataCacheName, payload, cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Distributed cache write timed out for {CacheKey}.", MasterDataCacheName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache write failed for {CacheKey}.", MasterDataCacheName);
            }
        }
    }
}
