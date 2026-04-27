using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Utilities;
using ASC.Web.Areas.Configuration.Models;
using ASC.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace ASC.Web.Areas.Configuration.Controllers
{
    [Area("Configuration")]
    [Authorize(Roles = "Admin")]
    public class MasterDataController : BaseController
    {
        private readonly IMasterDataOperations _masterData;
        private readonly IMasterDataCacheOperations _masterDataCache;
        private readonly IMapper _mapper;

        public MasterDataController(
            IMasterDataOperations masterDataOperations,
            IMasterDataCacheOperations masterDataCacheOperations,
            IMapper mapper)
        {
            _masterData = masterDataOperations;
            _masterDataCache = masterDataCacheOperations;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> MasterKeys()
        {
            var masterKeys = await _masterData.GetAllMasterKeysAsync();
            var viewModel = _mapper.Map<List<MasterDataKeyViewModel>>(masterKeys);

            HttpContext.Session.SetObject("MasterKeys", viewModel);

            return View(new MasterKeysViewModel
            {
                MasterKeys = viewModel,
                MasterKeyInContext = new MasterDataKeyViewModel(),
                IsEdit = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterKeys(MasterKeysViewModel model)
        {
            model.MasterKeys = HttpContext.Session.GetObject<List<MasterDataKeyViewModel>>("MasterKeys") ?? new List<MasterDataKeyViewModel>();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var masterKey = _mapper.Map<MasterDataKey>(model.MasterKeyInContext);
            var userName = User?.Identity?.Name ?? "System";
            var now = DateTime.UtcNow;

            if (model.IsEdit)
            {
                if (string.IsNullOrWhiteSpace(model.MasterKeyInContext.PartitionKey))
                {
                    ModelState.AddModelError("MasterKeyInContext.PartitionKey", "Missing partition key for update.");
                    return View(model);
                }

                masterKey.UpdatedBy = userName;
                masterKey.UpdatedDate = now;
                await _masterData.UpdateMasterKeyAsync(model.MasterKeyInContext.PartitionKey, masterKey);
            }
            else
            {
                var duplicated = await _masterData.GetMasterKeyByNameAsync(masterKey.Key);
                if (duplicated.Any())
                {
                    ModelState.AddModelError("MasterKeyInContext.Name", "Master key already exists.");
                    return View(model);
                }

                masterKey.Id = Guid.NewGuid().ToString();
                masterKey.CreatedBy = userName;
                masterKey.CreatedDate = now;
                masterKey.UpdatedBy = userName;
                masterKey.UpdatedDate = now;

                await _masterData.InsertMasterKeyAsync(masterKey);
            }

            await _masterDataCache.CreateMasterDataCacheAsync();
            return RedirectToAction(nameof(MasterKeys));
        }

        [HttpGet]
        public async Task<IActionResult> MasterValues()
        {
            var keys = await _masterData.GetAllMasterKeysAsync();
            ViewBag.MasterKeys = _mapper.Map<List<MasterDataKeyViewModel>>(keys);

            return View(new MasterValuesViewModel
            {
                MasterValues = new List<MasterDataValueViewModel>(),
                MasterValueInContext = new MasterDataValueViewModel(),
                IsEdit = false
            });
        }

        [HttpGet]
        public async Task<IActionResult> MasterValuesByKey(string key)
        {
            var values = await _masterData.GetAllMasterValuesByKeyAsync(key);
            var viewModels = _mapper.Map<List<MasterDataValueViewModel>>(values);

            foreach (var item in viewModels)
            {
                item.PartitionKey = key;
            }

            return Json(new { data = viewModels });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterValues(bool isEdit, MasterDataValueViewModel masterValue)
        {
            if (!ModelState.IsValid)
            {
                return Json("Error");
            }

            var key = (await _masterData.GetMasterKeyByNameAsync(masterValue.PartitionKey)).FirstOrDefault();
            if (key is null)
            {
                return Json(new { Error = true, Text = "Partition key does not exist." });
            }

            var now = DateTime.UtcNow;
            var userName = User?.Identity?.Name ?? "System";
            var entity = _mapper.Map<MasterDataValue>(masterValue);
            entity.MasterDataKeyId = key.Id;
            entity.Description = masterValue.Name;
            entity.UpdatedBy = userName;
            entity.UpdatedDate = now;

            if (isEdit)
            {
                if (string.IsNullOrWhiteSpace(masterValue.RowKey))
                {
                    return Json(new { Error = true, Text = "Missing RowKey for update." });
                }

                entity.DisplayOrder = (await _masterData.GetAllMasterValuesByKeyAsync(masterValue.PartitionKey))
                    .FirstOrDefault(x => x.Id == masterValue.RowKey)?.DisplayOrder ?? 0;

                await _masterData.UpdateMasterValueAsync(masterValue.PartitionKey, masterValue.RowKey, entity);
            }
            else
            {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreatedBy = userName;
                entity.CreatedDate = now;
                entity.DisplayOrder = (await _masterData.GetAllMasterValuesByKeyAsync(masterValue.PartitionKey)).Count + 1;
                await _masterData.InsertMasterValueAsync(entity);
            }

            await _masterDataCache.CreateMasterDataCacheAsync();
            return Json(true);
        }

        private async Task<List<MasterDataValue>> ParseMasterDataExcel(IFormFile excelFile)
        {
            var masterValueList = new List<MasterDataValue>();
            using (var memoryStream = new MemoryStream())
            {
                await excelFile.CopyToAsync(memoryStream);
                using (var package = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (var row = 2; row <= rowCount; row++)
                    {
                        var partitionKey = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var name = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        var isActiveText = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                        if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        var isActive = bool.TryParse(isActiveText, out var parsedIsActive) && parsedIsActive;

                        masterValueList.Add(new MasterDataValue
                        {
                            Id = Guid.NewGuid().ToString(),
                            MasterDataKeyId = partitionKey,
                            Value = name,
                            Description = name,
                            IsActive = isActive,
                            DisplayOrder = row - 1,
                            CreatedBy = User?.Identity?.Name ?? "System",
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = User?.Identity?.Name ?? "System",
                            UpdatedDate = DateTime.UtcNow
                        });
                    }
                }
            }

            return masterValueList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel()
        {
            var files = Request.Form.Files;
            if (!files.Any())
            {
                return Json(new { Error = true, Text = "Upload a file." });
            }

            var excelFile = files.First();
            if (excelFile.Length <= 0)
            {
                return Json(new { Error = true, Text = "Upload a file." });
            }

            try
            {
                var masterData = await ParseMasterDataExcel(excelFile);
                var result = await _masterData.UploadBulkMasterData(masterData);
                if (result)
                {
                    await _masterDataCache.CreateMasterDataCacheAsync();
                }

                return Json(new { Success = result });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Text = ex.Message });
            }
        }
    }
}
