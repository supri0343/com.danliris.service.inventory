using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessories : StandardEntity, IValidatableObject
    {
        public string ExpenditureNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureDestination { get; set; }

        public long UnitExpenditureId { get; set; }
        public string UnitExpenditureCode { get; set; }
        public string UnitExpenditureName { get; set; }

        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }

        public string LocalSalesNoteNo { get; set; }
        public int LocalSalesNoteId { get; set; }

        public string EtcRemark { get; set; }
        public string Remark { get; set; }

        public bool IsUsed { get; set; }

        public virtual ICollection<GarmentLeftoverWarehouseExpenditureAccessoriesItem> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
