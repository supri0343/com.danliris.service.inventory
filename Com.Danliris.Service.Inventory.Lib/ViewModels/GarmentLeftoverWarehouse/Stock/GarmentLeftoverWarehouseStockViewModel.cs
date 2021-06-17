using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockViewModel : BasicViewModel
    {
        public GarmentLeftoverWarehouseStockReferenceTypeEnum ReferenceType { get; set; }

        public UnitViewModel Unit { get; set; }

        public string PONo { get; set; }

        public string RONo { get; set; }

        public ProductViewModel Product { get; set; }
        public UomViewModel Uom { get; set; }
        public LeftoverComodityViewModel LeftoverComodity { get; set; }

        public double Quantity { get; set; }

        public List<GarmentLeftoverWarehouseStockHistoryViewModel> Histories { get; set; }
        public double BasicPrice { get; set; }
    }
}
