using Com.Moonlay.Models;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels
{
    public class GarmentLeftoverWarehouseReceiptFabricItem : StandardEntity
    {
        public int GarmentLeftoverWarehouseReceiptFabricId { get; set; }
        public GarmentLeftoverWarehouseReceiptFabric GarmentLeftoverWarehouseReceiptFabric { get; set; }

        public long UENItemId { get; set; }

        public string POSerialNumber { get; set; }

        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public string FabricRemark { get; set; }
        public string Composition { get; set; }

        public double BasicPrice { get; set; }

        public double Quantity { get; set; }

        public long UomId { get; set; }
        public string UomUnit { get; set; }

    }
}
