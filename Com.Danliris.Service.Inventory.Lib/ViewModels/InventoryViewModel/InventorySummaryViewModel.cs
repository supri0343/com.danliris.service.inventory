using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;


namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel
{
    public class InventorySummaryViewModel : BasicViewModel
    {
        public string code { get; set; }
        public int productId { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public int storageId { get; set; }
        public string storageCode { get; set; }
        public string storageName { get; set; }
        public double quantity { get; set; }
        public int uomId { get; set; }
        public string uom { get; set; }
        public string stockPlanning { get; set; }
    }
}