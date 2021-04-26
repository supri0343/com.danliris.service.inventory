using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels
{
    public class GarmentLeftoverWarehouseReceiptFinishedGood : StandardEntity, IValidatableObject
    {
        public string FinishedGoodReceiptNo { get; set; }
        public long UnitFromId { get; set; }
        public string UnitFromCode { get; set; }
        public string UnitFromName { get; set; }
        public string Invoice { get; set; }
        public string ContractNo { get; set; }
        public double Carton { get; set; }
        public string ExpenditureDesc { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string Description { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public virtual ICollection<GarmentLeftoverWarehouseReceiptFinishedGoodItem> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
