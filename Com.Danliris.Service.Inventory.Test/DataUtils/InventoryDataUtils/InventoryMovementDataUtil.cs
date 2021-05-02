using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using System;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils
{
    public class InventoryMovementDataUtil
    {
        private readonly InventoryMovementService Service;

        public InventoryMovementDataUtil(InventoryMovementService service)
        {
            Service = service;
        }

        public InventoryMovement GetNewData()
        {
            return new InventoryMovement
            {
                No = "No1",
                Date = DateTimeOffset.Now,
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",
                ReferenceNo = "Test001",
                ReferenceType = "TestType",
                ProductCode = "ProductCode1",
                ProductName = "ProductName1",
                UomUnit = "UomUnit1",
                StockPlanning = 1,
                Before = 1,
                Quantity = 1,
                After = 1,
                Type = "IN",
                Remark = "Remark",
            };
        }

        public InventoryMovementViewModel GetNewDataViewModel()
        {
            return new InventoryMovementViewModel
            {
                date = DateTimeOffset.Now,
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",
                referenceNo = "Test001",
                referenceType = "TestType",
                productCode = "ProductCode1",
                productName = "ProductName1",
                uomUnit = "UomUnit1",
                stockPlanning = 1,
                before = 1,
                quantity = 1,
                after = 1,
                type = "IN",
                remark = "Remark",
            };
        }

        public async Task<InventoryMovement> GetTestData()
        {
            InventoryMovement invMov = GetNewData();

            await Service.Create(invMov);

            return invMov;
        }

        public async Task<InventoryMovement> GetTestDataOUT()
        {
            InventoryMovement invMov = GetNewData();
            invMov.Type = "OUT";

            await Service.Create(invMov);

            return invMov;
        }
    }
}
