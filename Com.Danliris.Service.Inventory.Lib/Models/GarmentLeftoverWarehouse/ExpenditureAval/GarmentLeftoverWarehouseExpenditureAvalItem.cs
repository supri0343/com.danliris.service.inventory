using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAvalItem : StandardEntity
    {
        public int StockId { get; set; }

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string AvalReceiptNo { get; set; }
        public int AvalReceiptId { get; set; }
        public double Quantity { get; set; }
        public double ActualQuantity { get; set; }
        public int AvalExpenditureId { get; set; }
        public long UomId { get; set; }
        public string UomUnit { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public virtual GarmentLeftoverWarehouseExpenditureAval GarmentLeftoverWarehouseExpenditureAval { get; set; }
    }
}
