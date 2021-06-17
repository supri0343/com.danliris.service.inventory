using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class GarmentLeftoverWarehouseReceiptAccessoryItem: StandardEntity
    {
        public string POSerialNumber { get; set; }
        public long UENItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public long UomUnitId { get; set; }
        public string UomUnit { get; set; }
        public string ROJob { get; set; }
        public string Remark { get; set; }
        public double BasicPrice { get; set; }
        public int GarmentLeftOverWarehouseReceiptAccessoriesId { get; set; }
        [ForeignKey("GarmentLeftOverWarehouseReceiptAccessoriesId")]
        public virtual GarmentLeftoverWarehouseReceiptAccessory GarmentLeftoverWarehouseExpenditureAccessories { get; set; }
    }
}
