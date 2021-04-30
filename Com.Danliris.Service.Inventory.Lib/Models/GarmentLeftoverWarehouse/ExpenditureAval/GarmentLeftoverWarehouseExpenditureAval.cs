using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAval : StandardEntity, IValidatableObject
    {
        public string AvalExpenditureNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureTo { get; set; }
        public string AvalType { get; set; }
        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string OtherDescription { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public int LocalSalesNoteId { get; set; }
        public string Description { get; set; }
        public virtual ICollection<GarmentLeftoverWarehouseExpenditureAvalItem> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
