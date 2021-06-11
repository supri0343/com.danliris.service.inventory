using Com.Danliris.Service.Inventory.Lib.Helpers;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels
{
    public class GarmentLeftoverWarehouseReceiptFabricItemViewModel : BasicViewModel
    {
        public int GarmentLeftoverWarehouseReceiptFabricId { get; set; }

        public long UENItemId { get; set; }

        public string POSerialNumber { get; set; }

        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public string FabricRemark { get; set; }

        public string Composition { get; set; }

        public double BasicPrice { get; set; }

        public double Quantity { get; set; }

        public UomViewModel Uom { get; set; }
    }
}
