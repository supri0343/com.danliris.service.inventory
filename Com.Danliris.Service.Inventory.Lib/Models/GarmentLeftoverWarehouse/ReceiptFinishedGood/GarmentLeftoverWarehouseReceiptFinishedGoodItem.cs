using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodItem : StandardEntity
    {
        public Guid ExpenditureGoodItemId { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public Guid ExpenditureGoodId { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public long ComodityId { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityName { get; set; }
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
        public double BasicPrice { get; set; }
        public virtual GarmentLeftoverWarehouseReceiptFinishedGood GarmentLeftoverWarehouseReceiptFinishedGood { get; set; }
    }
}
