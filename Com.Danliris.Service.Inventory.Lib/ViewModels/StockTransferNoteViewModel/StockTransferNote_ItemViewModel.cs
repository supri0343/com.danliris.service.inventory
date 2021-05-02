using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel
{
    public class StockTransferNote_ItemViewModel : BasicViewModel, IValidatableObject
    {
        public int StockTransferNoteId { get; set; }
        public InventorySummaryViewModel Summary { get; set; }
        public double? TransferedQuantity { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
