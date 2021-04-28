using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingDocumentDetailViewModel : BasicViewModel
    {
        public DateTimeOffset Date { get; set; }
        public string BonNo { get; set; }
        public string BonType { get; set; }
        public int StorageId { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public string Type { get; set; }
        public ICollection<InventoryWeavingItemDetailViewModel> Detail{ get; set; }
    }
}
