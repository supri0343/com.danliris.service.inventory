using Com.Danliris.Service.Inventory.Lib.Helpers;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class GarmentLeftoverWarehouseExpenditureFabricItemViewModel : BasicViewModel
    {
        public int StockId { get; set; }

        public UnitViewModel Unit { get; set; }

        public string PONo { get; set; }

        public double Quantity { get; set; }

        public UomViewModel Uom { get; set; }
        public double BasicPrice { get; set; }
    }
}
