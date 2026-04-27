using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ASC.Business;
using ASC.Business.Interfaces;
using ASC.Model;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VSDiagnostics;

namespace ASC.Business.Benchmarks;
[CPUUsageDiagnoser]
public class MasterDataCacheOperationsBenchmark
{
    private MasterDataCacheOperations _sut = null!;
    [GlobalSetup]
    public void Setup()
    {
        var cache = new InMemoryDistributedCache();
        var masterData = new FakeMasterDataOperations(500, 5000);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        _sut = new MasterDataCacheOperations(cache, masterData, memoryCache, NullLogger<MasterDataCacheOperations>.Instance);
    }

    [Benchmark]
    public Task CreateMasterDataCacheAsync()
    {
        return _sut.CreateMasterDataCacheAsync();
    }

    private sealed class FakeMasterDataOperations : IMasterDataOperations
    {
        private readonly List<MasterDataKey> _keys;
        private readonly List<MasterDataValue> _values;
        public FakeMasterDataOperations(int keyCount, int valueCount)
        {
            _keys = Enumerable.Range(1, keyCount).Select(i => new MasterDataKey { Id = $"K{i}", Key = $"Key{i}", Description = $"Key{i}", IsActive = true }).ToList();
            _values = Enumerable.Range(1, valueCount).Select(i => new MasterDataValue { Id = $"V{i}", MasterDataKeyId = _keys[(i - 1) % _keys.Count].Id, Value = $"Value{i}", IsActive = true }).ToList();
        }

        public Task<List<MasterDataKey>> GetAllMasterKeysAsync() => Task.FromResult(_keys);
        public Task<List<MasterDataKey>> GetMasterKeyByNameAsync(string name) => Task.FromResult(_keys.Where(k => k.Key == name).ToList());
        public Task<bool> InsertMasterKeyAsync(MasterDataKey key) => Task.FromResult(true);
        public Task<bool> UpdateMasterKeyAsync(string originalPartitionKey, MasterDataKey key) => Task.FromResult(true);
        public Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key) => Task.FromResult(_values.Where(v => v.MasterDataKeyId == key).ToList());
        public Task<List<MasterDataValue>> GetAllMasterValuesAsync() => Task.FromResult(_values);
        public Task<MasterDataValue?> GetMasterValueByNameAsync(string key, string name) => Task.FromResult(_values.FirstOrDefault(v => v.MasterDataKeyId == key && v.Value == name));
        public Task<bool> InsertMasterValueAsync(MasterDataValue value) => Task.FromResult(true);
        public Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string originalRowKey, MasterDataValue value) => Task.FromResult(true);
        public Task<bool> UploadBulkMasterData(List<MasterDataValue> values) => Task.FromResult(true);
    }

    private sealed class InMemoryDistributedCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> _store = new(StringComparer.Ordinal);
        public byte[]? Get(string key) => _store.TryGetValue(key, out var value) ? value : null;
        public Task<byte[]?> GetAsync(string key, CancellationToken token = default) => Task.FromResult(Get(key));
        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => _store[key] = value;
        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }
    }
}
