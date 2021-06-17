using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel : BasicViewModel
    {
        public Guid ExpenditureGoodItemId { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public Guid ExpenditureGoodId { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public ComodityViewModel Comodity { get; set; }
        public SizeViewModel Size { get; set; }
        public UomViewModel Uom { get; set; }
        public string Remark { get; set; }
        public double Quantity { get; set; }
        public int FinishedGoodReceiptId { get; set; }
        public LeftoverComodityViewModel LeftoverComodity { get; set; }
        public double BasicPrice { get; set; }
    }
}
