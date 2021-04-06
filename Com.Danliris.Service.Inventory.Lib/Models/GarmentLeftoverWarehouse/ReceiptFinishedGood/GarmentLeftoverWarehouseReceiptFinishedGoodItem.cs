using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodItem : StandardEntity
    {
        public Guid ExpenditureGoodItemId { get; set; }
        public long SizeId { get; set; }
        public string SizeName { get; set; }
        public string UomUnit { get; set; }
        public long UomId { get; set; }
        public double  Quantity { get; set; }
        public string Remark { get; set; }
        public int FinishedGoodReceiptId { get; set; }
        public long LeftoverComodityId { get; set; }
        public string LeftoverComodityCode { get; set; }
        public string LeftoverComodityName { get; set; }
        public virtual GarmentLeftoverWarehouseReceiptFinishedGood GarmentLeftoverWarehouseReceiptFinishedGood { get; set; }
    }
}
