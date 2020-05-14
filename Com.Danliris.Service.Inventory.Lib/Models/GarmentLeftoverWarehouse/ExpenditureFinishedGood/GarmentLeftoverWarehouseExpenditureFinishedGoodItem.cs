using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodItem : StandardEntity
    {

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string RONo { get; set; }
        public double ExpenditureQuantity { get; set; }
        public int FinishedGoodExpenditureId { get; set; }
        public virtual GarmentLeftoverWarehouseExpenditureFinishedGood GarmentLeftoverWarehouseExpenditureFinishedGood { get; set; }
    }
}
