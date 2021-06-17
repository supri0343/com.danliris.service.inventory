using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel
    {
        public long UENItemId { get; set; }
        public string POSerialNumber { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public long UomUnitId { get; set; }
        public string UomUnit { get; set; }
        public string ROJob { get; set; }
        public string Remark { get; set; }
        public ProductViewModel Product { get; set; }
        public UomViewModel Uom { get; set; }
        public double BasicPrice { get; set; }
    }
}
