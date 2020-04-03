using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels
{
    public class GarmentLeftoverWarehouseReceiptAvalViewModel : BasicViewModel, IValidatableObject
    {
        public string AvalReceiptNo { get; set; }

        public UnitViewModel UnitFrom { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public string AvalType { get; set; }
        public string Remark { get; set; }
        public List<GarmentLeftoverWarehouseReceiptAvalItemViewModel> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
