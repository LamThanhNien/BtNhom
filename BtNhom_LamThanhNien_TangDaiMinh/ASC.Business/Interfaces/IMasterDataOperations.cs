using ASC.Model;

namespace ASC.Business.Interfaces
{
    public interface IMasterDataOperations
    {
        Task<List<MasterDataKey>> GetAllMasterKeysAsync();
        Task<List<MasterDataKey>> GetMasterKeyByNameAsync(string name);
        Task<bool> InsertMasterKeyAsync(MasterDataKey key);
        Task<bool> UpdateMasterKeyAsync(string originalPartitionKey, MasterDataKey key);

        Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key);
        Task<List<MasterDataValue>> GetAllMasterValuesAsync();
        Task<MasterDataValue?> GetMasterValueByNameAsync(string key, string name);
        Task<bool> InsertMasterValueAsync(MasterDataValue value);
        Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string originalRowKey, MasterDataValue value);
        Task<bool> UploadBulkMasterData(List<MasterDataValue> values);
    }
}
