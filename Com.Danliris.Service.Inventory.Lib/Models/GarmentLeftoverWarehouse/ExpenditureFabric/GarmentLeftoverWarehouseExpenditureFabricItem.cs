using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class GarmentLeftoverWarehouseExpenditureFabricItem : StandardEntity
    {
        public int ExpenditureId { get; set; }
        public GarmentLeftoverWarehouseExpenditureFabric GarmentLeftoverWarehouseExpenditureFabric { get; set; }

        public int StockId { get; set; }

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public string PONo { get; set; }

        public double Quantity { get; set; }

        public long UomId { get; set; }
        public string UomUnit { get; set; }

        public double BasicPrice { get; set; }
    }
}
