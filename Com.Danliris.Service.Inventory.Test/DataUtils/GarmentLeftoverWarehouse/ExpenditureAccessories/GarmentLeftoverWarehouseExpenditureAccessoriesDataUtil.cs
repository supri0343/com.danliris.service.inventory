using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil
    {
        private readonly GarmentLeftoverWarehouseExpenditureAccessoriesService Service;

        public GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil(GarmentLeftoverWarehouseExpenditureAccessoriesService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseExpenditureAccessories GetNewData()
        {
            return new GarmentLeftoverWarehouseExpenditureAccessories
            {
                ExpenditureDate = DateTimeOffset.Now,
                ExpenditureDestination = "SAMPLE",
                Remark = "Remark",
                LocalSalesNoteNo = "LocalSalesNoteNo",
                IsUsed = false,
                Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItem>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItem
                    {
                        StockId = 1,
                        UnitId = 1,
                        UnitCode = "Unit",
                        UnitName = "Unit",
                        PONo = "PONo",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom"
                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseExpenditureAccessories> GetTestData()
        {
            GarmentLeftoverWarehouseExpenditureAccessories data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseExpenditureAccessories CopyData(GarmentLeftoverWarehouseExpenditureAccessories oldData)
        {
            GarmentLeftoverWarehouseExpenditureAccessories newData = new GarmentLeftoverWarehouseExpenditureAccessories();

            PropertyCopier<GarmentLeftoverWarehouseExpenditureAccessories, GarmentLeftoverWarehouseExpenditureAccessories>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseExpenditureAccessoriesItem newItem = new GarmentLeftoverWarehouseExpenditureAccessoriesItem();

                PropertyCopier<GarmentLeftoverWarehouseExpenditureAccessoriesItem, GarmentLeftoverWarehouseExpenditureAccessoriesItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
