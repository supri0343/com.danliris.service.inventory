using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class GarmentLeftoverWarehouseExpenditureFabricDataUtil
    {
        private readonly GarmentLeftoverWarehouseExpenditureFabricService Service;

        public GarmentLeftoverWarehouseExpenditureFabricDataUtil(GarmentLeftoverWarehouseExpenditureFabricService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseExpenditureFabric GetNewData()
        {
            return new GarmentLeftoverWarehouseExpenditureFabric
            {
                ExpenditureDate = DateTimeOffset.Now,
                ExpenditureDestination = "SAMPLE",
                Remark = "Remark",
                LocalSalesNoteNo = "LocalSalesNoteNo",
                IsUsed=false,
                Items = new List<GarmentLeftoverWarehouseExpenditureFabricItem>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItem
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

        public async Task<GarmentLeftoverWarehouseExpenditureFabric> GetTestData()
        {
            GarmentLeftoverWarehouseExpenditureFabric data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseExpenditureFabric CopyData(GarmentLeftoverWarehouseExpenditureFabric oldData)
        {
            GarmentLeftoverWarehouseExpenditureFabric newData = new GarmentLeftoverWarehouseExpenditureFabric();

            PropertyCopier<GarmentLeftoverWarehouseExpenditureFabric, GarmentLeftoverWarehouseExpenditureFabric>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseExpenditureFabricItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseExpenditureFabricItem newItem = new GarmentLeftoverWarehouseExpenditureFabricItem();

                PropertyCopier<GarmentLeftoverWarehouseExpenditureFabricItem, GarmentLeftoverWarehouseExpenditureFabricItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
