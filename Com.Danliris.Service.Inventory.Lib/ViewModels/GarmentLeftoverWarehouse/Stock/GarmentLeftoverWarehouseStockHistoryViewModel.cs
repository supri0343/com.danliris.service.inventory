using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockHistoryViewModel : BasicViewModel
    {
        public string StockReferenceNo { get; set; }
        public int StockReferenceId { get; set; }
        public int StockReferenceItemId { get; set; }

        public GarmentLeftoverWarehouseStockTypeEnum StockType { get; set; }

        public double BeforeQuantity { get; set; }
        public double Quantity { get; set; }
        public double AfterQuantity { get; set; }
    }
}
