using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel
{
    public class InventoryWeavingDocument : StandardEntity, IValidatableObject
    {
        
        public DateTimeOffset Date { get; set; }
        public string BonNo { get; set; }
        public string BonType { get; set; }
        

        public int StorageId { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public string Remark { get; set; }
        public string Type { get; set; }
        public virtual ICollection<InventoryWeavingDocumentItem> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
