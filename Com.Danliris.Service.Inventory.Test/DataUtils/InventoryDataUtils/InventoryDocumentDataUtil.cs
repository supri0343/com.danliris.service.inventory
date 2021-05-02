using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils
{
    public class InventoryDocumentDataUtil
    {
        private readonly InventoryDocumentService Service;
        public InventoryDocumentDataUtil(InventoryDocumentService service)
        {
            Service = service;
        }

        public InventoryDocument GetNewData()
        {
            return new InventoryDocument
            {
                No = "No1",
                Date = DateTimeOffset.Now,
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",
                ReferenceNo = "Test001",
                ReferenceType = "TestType",
                Type = "IN",
                Remark = "Remark",
                Items = new List<InventoryDocumentItem> { new InventoryDocumentItem(){
                    ProductId = 1,
                    ProductCode = "ProductCode",
                    ProductName = "ProductName",
                    Quantity = 10,
                    UomId = 1,
                    UomUnit = "Uom",
                    StockPlanning=0,
                } }
            };
        }

        public InventoryDocumentViewModel GetNewDataViewModel()
        {
            return new InventoryDocumentViewModel
            {
                date = DateTimeOffset.Now,
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",
                referenceNo = "Test001",
                referenceType = "TestType",
                type = "IN",
                remark = "Remark",
                items = new List<InventoryDocumentItemViewModel> { new InventoryDocumentItemViewModel(){
                    productCode = "ProductCode",
                    productName = "ProductName",
                    uomId = 1,
                    uom = "Uom",
                    quantity=10,
                    stockPlanning=0,
                } }
            };
        }

        public async Task<InventoryDocument> GetTestData()
        {
            InventoryDocument invDoc = GetNewData();

            await Service.Create(invDoc);

            return invDoc;
        }

        public async Task<InventoryDocument> GetTestDataOUT()
        {
            InventoryDocument invDoc = GetNewData();
            invDoc.Type = "OUT";

            await Service.Create(invDoc);

            return invDoc;
        }
    }
}
