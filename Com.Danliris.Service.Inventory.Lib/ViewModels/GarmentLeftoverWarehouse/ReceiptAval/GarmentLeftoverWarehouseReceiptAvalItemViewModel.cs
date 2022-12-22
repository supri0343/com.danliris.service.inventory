using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels
{
    public class GarmentLeftoverWarehouseReceiptAvalItemViewModel : BasicViewModel
    {
        //Aval Component
        public string Article { get; set; }
        public Guid AvalComponentId { get; set; }
        public string AvalComponentNo { get; set; }

        //Fabric
        public int AvalReceiptId { get; set; }
        public Guid GarmentAvalProductId { get; set; }
        public Guid GarmentAvalProductItemId { get; set; }

        //Accecoris
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }

        //General
        public string RONo { get; set; }
        public double Quantity { get; set; }
        public UomViewModel Uom { get; set; }
        public string BCNo { get;  set; }
        public DateTimeOffset? BCDate { get;  set; }
        public string POSerialNumber { get;  set; }
        public string BCType { get;  set; }
    }
}
