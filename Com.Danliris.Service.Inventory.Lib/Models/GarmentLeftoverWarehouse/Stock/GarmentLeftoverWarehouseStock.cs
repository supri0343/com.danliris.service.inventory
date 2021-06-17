using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Moonlay.Models;
using System.Collections.Generic;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStock : StandardEntity
    {
        public GarmentLeftoverWarehouseStockReferenceTypeEnum ReferenceType { get; set; }

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public string PONo { get; set; }

        public string RONo { get; set; }

        public long? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public long? UomId { get; set; }
        public string UomUnit { get; set; }

        public long? LeftoverComodityId { get; set; }
        public string LeftoverComodityCode { get; set; }
        public string LeftoverComodityName { get; set; }

        public double Quantity { get; set; }

        public virtual ICollection<GarmentLeftoverWarehouseStockHistory> Histories { get; set; }

        public double BasicPrice { get; set; }
    }
}
