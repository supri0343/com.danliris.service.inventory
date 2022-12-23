using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste
{
    public class GarmentReceiptWasteProductions : StandardEntity, IValidatableObject
    {
        public string GarmentReceiptWasteNo { get; set; }
        public string SourceName { get; set; }
        public string DestinationName { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public string WasteType { get; set; }
        public string Remark { get; set; }
        public double TotalAval { get; set; }
        public bool IsUsed { get; set; }
        public virtual ICollection<GarmentReceiptWasteProductionItems> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
