using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.Inventory
{
    public interface IInventoryDocumentService : IMap<InventoryDocument, InventoryDocumentViewModel>
    {
        ReadResponse<InventoryDocument> Read(int page, int size, string order, string keyword, string filter);
        InventoryDocument ReadModelById(int id);
        Task<int> Create(InventoryDocument model);
        Task<int> CreateMulti(List<InventoryDocument> models);
    }
}
