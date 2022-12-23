using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste
{
    public class GarmentExpenditureWasteProductions : StandardEntity, IValidatableObject
    {
        public string GarmentExpenditureWasteNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureTo { get; set; }
        public string WasteType { get; set; }
        public string Description { get; set; }
        public string BCOutNo { get; set; }
        public string BCOutType { get; set; }
        public DateTimeOffset? BCOutDate { get; set; }
        public double ActualQuantity { get; set; }
        public virtual ICollection<GarmentExpenditureWasteProductionItems> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
