using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class GarmentLeftoverWarehouseReceiptAccessory: StandardEntity, IValidatableObject
    {
        public string InvoiceNoReceive { get; set; }
        public string RequestUnitCode { get; set; }
        public string RequestUnitName { get; set; }
        public long RequestUnitId { get; set; }
        public string UENNo { get; set; }
        public long UENid { get; set; }
        public string StorageFromCode { get; set; }
        public long StorageFromId { get; set; }
        public string StorageFromName { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public DateTimeOffset StorageReceiveDate { get; set; }
        public string Remark { get; set; }
        public List<GarmentLeftoverWarehouseReceiptAccessoryItem> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
