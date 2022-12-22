using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels
{
    public class GarmentLeftoverWarehouseReceiptAvalItem : StandardEntity
    {
        public Guid AvalComponentId { get; set; }
        public string AvalComponentNo { get; set; }
        public string Article { get; set; }
        public int AvalReceiptId { get; set; }
        public Guid GarmentAvalProductId { get; set; }
        public Guid GarmentAvalProductItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }

        public double Quantity { get; set; }

        public long UomId { get; set; }
        public string UomUnit { get; set; }
        public string RONo { get; set; }
        public string BCNo { get; set; }
        public DateTimeOffset? BCDate { get; set; }
        public string POSerialNumber { get; set; }
        public string BCType { get; set; }
        public GarmentLeftoverWarehouseReceiptAval GarmentLeftoverWarehouseReceiptAval { get; set; }
    }
}
