using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodDataUtils
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodDataUtil
    {
        private readonly GarmentLeftoverWarehouseReceiptFinishedGoodService Service;

        public GarmentLeftoverWarehouseReceiptFinishedGoodDataUtil(GarmentLeftoverWarehouseReceiptFinishedGoodService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseReceiptFinishedGood GetNewData()
        {
            
            return new GarmentLeftoverWarehouseReceiptFinishedGood
            {
                UnitFromId = 1,
                UnitFromCode = "Unit",
                UnitFromName = "Unit",
                ExpenditureDate = DateTimeOffset.Now,
                ReceiptDate = DateTimeOffset.Now,
                Description = "Remark",
                ExpenditureDesc="desc",
                Invoice="Invoice",
                Carton=11,
                ContractNo="ContractNo",
                Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItem>
                {
                    new GarmentLeftoverWarehouseReceiptFinishedGoodItem
                    {
                        ExpenditureGoodItemId = Guid.NewGuid(),
                        SizeId = 1,
                        SizeName = "SizeName",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom",
                        Remark="remark",
                        LeftoverComodityCode="como",
                        LeftoverComodityId=1,
                        LeftoverComodityName="como",
                        ExpenditureGoodId = Guid.NewGuid(),
                        ExpenditureGoodNo = "ExGoodNo",
                        BuyerId = 1,
                        BuyerCode = "BuyerCode",
                        BuyerName = "BuyerName",
                        ComodityCode= "ComodityCode",
                        ComodityId=1,
                        ComodityName= "ComodityName",
                        Article="art",
                        RONo="roNo",
                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseReceiptFinishedGood> GetTestData()
        {
            GarmentLeftoverWarehouseReceiptFinishedGood data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseReceiptFinishedGood CopyData(GarmentLeftoverWarehouseReceiptFinishedGood oldData)
        {
            GarmentLeftoverWarehouseReceiptFinishedGood newData = new GarmentLeftoverWarehouseReceiptFinishedGood();

            PropertyCopier<GarmentLeftoverWarehouseReceiptFinishedGood, GarmentLeftoverWarehouseReceiptFinishedGood>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseReceiptFinishedGoodItem newItem = new GarmentLeftoverWarehouseReceiptFinishedGoodItem();

                PropertyCopier<GarmentLeftoverWarehouseReceiptFinishedGoodItem, GarmentLeftoverWarehouseReceiptFinishedGoodItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
