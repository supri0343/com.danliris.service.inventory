using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public interface IInventoryWeavingDocumentOutService
    {
        ListResult<InventoryWeavingDocument> Read(int page, int size, string order, string keyword, string filter);
        ReadResponse<InventoryWeavingDocumentItem> GetDistinctMaterial(int page, int size, string filter, string order, string keyword);
        InventoryWeavingDocumentDetailViewModel ReadById(int id);
        List<InventoryWeavingItemDetailViewModel> GetMaterialItemList(string material);
        Task<InventoryWeavingDocument> MapToModel(InventoryWeavingDocumentOutViewModel data);
        Task Create(InventoryWeavingDocument model);
    }
}
