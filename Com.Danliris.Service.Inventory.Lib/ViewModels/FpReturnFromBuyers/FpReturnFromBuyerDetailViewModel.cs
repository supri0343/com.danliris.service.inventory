using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers
{
    public class FpReturnFromBuyerDetailViewModel : BasicViewModel, IValidatableObject
    {
        public IList<FpReturnFromBuyerItemViewModel> Items { get; set; }
        public ProductionOrderIntegrationViewModel ProductionOrder { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductionOrderIntegrationViewModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public double? OrderQuantity { get; set; }
        public bool IsCompleted { get; set; }
        public double? DistributedQuantity { get; set; }
        public OrderTypeIntegrationViewModel OrderType { get; set; }
    }

    public class OrderTypeIntegrationViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }
}
