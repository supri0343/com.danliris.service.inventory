using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Danliris.Service.Inventory.Test.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil
{
    public class StockTransferNoteDataUtil
    {
        private readonly NewStockTransferNoteService Service;

        public StockTransferNoteDataUtil(NewStockTransferNoteService service)
        {
            Service = service;
        }

        public StockTransferNoteViewModel GetEmptyData()
        {
            StockTransferNoteViewModel Data = new StockTransferNoteViewModel
            {
                ReferenceNo = string.Empty,
                ReferenceType = string.Empty,
                SourceStorage = new StorageViewModel(),
                TargetStorage = new StorageViewModel(),
                StockTransferNoteItems = new List<StockTransferNote_ItemViewModel> { new StockTransferNote_ItemViewModel() }
            };

            return Data;
        }

        public StockTransferNote GetNewData()
        {
            StockTransferNote TestData = new StockTransferNote
            {
                ReferenceNo = "Reference No Test",
                ReferenceType = "Reference Type Test",
                SourceStorageId = "1",
                SourceStorageCode = "code",
                SourceStorageName = "name",
                TargetStorageId = "1",
                TargetStorageCode = "code",
                TargetStorageName = "name",
                IsApproved = false,
                StockTransferNoteItems = new List<StockTransferNote_Item> { new StockTransferNote_Item(){
                    ProductCode = "code",
                    ProductId = "1",
                    ProductName = "name",
                    StockQuantity = 1,
                    TransferedQuantity = 1,
                    UomId = "1",
                    UomUnit = "unit"
                } }
            };

            return TestData;
        }


        public async Task<StockTransferNote> GetTestData()
        {
            StockTransferNote Data = GetNewData();
            await this.Service.CreateAsync(Data);
            return Data;
        }
    }
}
