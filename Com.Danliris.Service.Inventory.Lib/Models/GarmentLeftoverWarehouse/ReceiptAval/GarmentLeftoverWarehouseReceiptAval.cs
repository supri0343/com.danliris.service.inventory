using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels
{
    public class GarmentLeftoverWarehouseReceiptAval : StandardEntity, IValidatableObject
    {
        public string AvalReceiptNo { get; set; }
        public long UnitFromId { get; set; }
        public string UnitFromCode { get; set; }
        public string UnitFromName { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public string AvalType { get; set; }
        public string Remark { get; set; }
        public double TotalAval { get; set; }
        public bool IsUsed { get; set; }
        public virtual ICollection<GarmentLeftoverWarehouseReceiptAvalItem> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
