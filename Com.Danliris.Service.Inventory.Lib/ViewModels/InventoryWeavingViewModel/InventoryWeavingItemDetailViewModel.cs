using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingItemDetailViewModel : BasicViewModel
    {
        public string ProductOrderNo { get; set; }
        public string ReferenceNo { get; set; }
        public string Construction { get; set; }
        public string Year { get; set; }

        public ICollection<ItemListDetailViewModel> ListItems { get; set; }
    }
}
