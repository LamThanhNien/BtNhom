using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.Model;

namespace ASC.Business
{
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public MasterDataOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MasterDataKey>> GetAllMasterKeysAsync()
        {
            var masterKeys = await _unitOfWork.Repository<MasterDataKey>().GetAllAsync();
            return masterKeys
                .OrderBy(x => x.Key)
                .ToList();
        }

        public async Task<List<MasterDataKey>> GetMasterKeyByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<MasterDataKey>();
            }

            var masterKeys = await _unitOfWork.Repository<MasterDataKey>()
                .FindAllAsync(x => x.Key == name);

            return masterKeys.ToList();
        }

        public async Task<bool> InsertMasterKeyAsync(MasterDataKey key)
        {
            await _unitOfWork.Repository<MasterDataKey>().AddAsync(key);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> UpdateMasterKeyAsync(string originalPartitionKey, MasterDataKey key)
        {
            if (string.IsNullOrWhiteSpace(originalPartitionKey))
            {
                return false;
            }

            var masterKey = await _unitOfWork.Repository<MasterDataKey>()
                .FindAsync(x => x.Key == originalPartitionKey);

            if (masterKey is null)
            {
                return false;
            }

            masterKey.Key = key.Key;
            masterKey.Description = key.Description;
            masterKey.IsActive = key.IsActive;
            masterKey.UpdatedBy = key.UpdatedBy;
            masterKey.UpdatedDate = key.UpdatedDate;

            await _unitOfWork.Repository<MasterDataKey>().UpdateAsync(masterKey);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return new List<MasterDataValue>();
            }

            var masterKey = await _unitOfWork.Repository<MasterDataKey>()
                .FindAsync(x => x.Key == key);

            if (masterKey is null)
            {
                return new List<MasterDataValue>();
            }

            var values = await _unitOfWork.Repository<MasterDataValue>()
                .FindAllAsync(x => x.MasterDataKeyId == masterKey.Id);

            return values
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Value)
                .ToList();
        }

        public async Task<List<MasterDataValue>> GetAllMasterValuesAsync()
        {
            var values = await _unitOfWork.Repository<MasterDataValue>().GetAllAsync();
            return values
                .OrderBy(x => x.Value)
                .ToList();
        }

        public async Task<MasterDataValue?> GetMasterValueByNameAsync(string key, string name)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var masterKey = await _unitOfWork.Repository<MasterDataKey>()
                .FindAsync(x => x.Key == key);

            if (masterKey is null)
            {
                return null;
            }

            return await _unitOfWork.Repository<MasterDataValue>()
                .FindAsync(x => x.MasterDataKeyId == masterKey.Id && x.Value == name);
        }

        public async Task<bool> InsertMasterValueAsync(MasterDataValue value)
        {
            await _unitOfWork.Repository<MasterDataValue>().AddAsync(value);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string originalRowKey, MasterDataValue value)
        {
            if (string.IsNullOrWhiteSpace(originalPartitionKey) || string.IsNullOrWhiteSpace(originalRowKey))
            {
                return false;
            }

            var masterKey = await _unitOfWork.Repository<MasterDataKey>()
                .FindAsync(x => x.Key == originalPartitionKey);
            if (masterKey is null)
            {
                return false;
            }

            var masterValue = await _unitOfWork.Repository<MasterDataValue>()
                .FindAsync(x => x.Id == originalRowKey && x.MasterDataKeyId == masterKey.Id);
            if (masterValue is null)
            {
                return false;
            }

            masterValue.Value = value.Value;
            masterValue.Description = value.Description;
            masterValue.IsActive = value.IsActive;
            masterValue.DisplayOrder = value.DisplayOrder;
            masterValue.UpdatedBy = value.UpdatedBy;
            masterValue.UpdatedDate = value.UpdatedDate;

            await _unitOfWork.Repository<MasterDataValue>().UpdateAsync(masterValue);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> UploadBulkMasterData(List<MasterDataValue> values)
        {
            if (values is null || values.Count == 0)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value.MasterDataKeyId) || string.IsNullOrWhiteSpace(value.Value))
                {
                    continue;
                }

                // During Excel parsing, MasterDataKeyId temporarily stores partition key text.
                var partitionKey = value.MasterDataKeyId.Trim();
                var masterKey = (await GetMasterKeyByNameAsync(partitionKey)).FirstOrDefault();
                if (masterKey is null)
                {
                    masterKey = new MasterDataKey
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = partitionKey,
                        Description = partitionKey,
                        CreatedBy = value.CreatedBy,
                        CreatedDate = value.CreatedDate,
                        UpdatedBy = value.UpdatedBy,
                        UpdatedDate = value.UpdatedDate
                    };
                    await _unitOfWork.Repository<MasterDataKey>().AddAsync(masterKey);
                    await _unitOfWork.CommitAsync();
                }

                var existingValue = await _unitOfWork.Repository<MasterDataValue>()
                    .FindAsync(x => x.MasterDataKeyId == masterKey.Id && x.Value == value.Value);

                if (existingValue is null)
                {
                    value.MasterDataKeyId = masterKey.Id;
                    await _unitOfWork.Repository<MasterDataValue>().AddAsync(value);
                }
                else
                {
                    existingValue.IsActive = value.IsActive;
                    existingValue.Description = value.Description;
                    existingValue.DisplayOrder = value.DisplayOrder;
                    existingValue.UpdatedBy = value.UpdatedBy;
                    existingValue.UpdatedDate = value.UpdatedDate;
                    await _unitOfWork.Repository<MasterDataValue>().UpdateAsync(existingValue);
                }
            }

            return await _unitOfWork.CommitAsync() > 0;
        }
    }
}
