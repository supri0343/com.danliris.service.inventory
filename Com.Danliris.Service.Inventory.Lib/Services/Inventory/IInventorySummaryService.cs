using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.Inventory
{
    public interface IInventorySummaryService : IMap<InventorySummary, InventorySummaryViewModel>
    {
        Task<int> Create(InventorySummary model);
        InventorySummary ReadModelById(int id);
        ReadResponse<InventorySummary> Read(int page, int size, string order, string keyword, string filter);
        List<InventorySummary> GetByStorageAndMTR(string storageName);
        Tuple<List<InventorySummaryViewModel>, int> GetReport(string storageCode, string productCode, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(string storageCode, string productCode, int offset);
        List<InventorySummaryViewModel> GetInventorySummaries(string productIds = "{}");
        InventorySummary GetSummaryByParams(int storageId, int productId, int uomId);
    }
}
