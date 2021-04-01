using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodDataUtil
    {
        private readonly GarmentLeftoverWarehouseExpenditureFinishedGoodService Service;

        public GarmentLeftoverWarehouseExpenditureFinishedGoodDataUtil(GarmentLeftoverWarehouseExpenditureFinishedGoodService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseExpenditureFinishedGood GetNewData()
        {

            return new GarmentLeftoverWarehouseExpenditureFinishedGood
            {
                BuyerId = 1,
                BuyerCode = "BuyerCode",
                BuyerName = "BuyerName",
                ExpenditureDate = DateTimeOffset.Now,
                ExpenditureTo = "JUAL LOKAL",
                Description = "Remark",
                LocalSalesNoteNo = "LocalSalesNoteNo",
                OtherDescription="Lain-lain",
                Consignment = false,
                Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItem>
                {
                    new GarmentLeftoverWarehouseExpenditureFinishedGoodItem
                    {
                        UnitId = 1,
                        UnitCode = "Unit",
                        UnitName = "Unit",
                        RONo="ro",
                        ExpenditureQuantity=1
                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseExpenditureFinishedGood> GetTestData()
        {
            GarmentLeftoverWarehouseExpenditureFinishedGood data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseExpenditureFinishedGood CopyData(GarmentLeftoverWarehouseExpenditureFinishedGood oldData)
        {
            GarmentLeftoverWarehouseExpenditureFinishedGood newData = new GarmentLeftoverWarehouseExpenditureFinishedGood();

            PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGood, GarmentLeftoverWarehouseExpenditureFinishedGood>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseExpenditureFinishedGoodItem newItem = new GarmentLeftoverWarehouseExpenditureFinishedGoodItem();

                PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodItem, GarmentLeftoverWarehouseExpenditureFinishedGoodItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
