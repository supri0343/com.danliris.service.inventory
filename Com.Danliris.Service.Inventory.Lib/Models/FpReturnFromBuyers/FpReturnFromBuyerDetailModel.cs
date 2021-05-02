using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers
{
    public class FpReturnFromBuyerDetailModel : StandardEntity, IValidatableObject
    {
        public ICollection<FpReturnFromBuyerItemModel> Items { get; set; }
        //public ProductionOrderViewModel ProductionOrder { get; set; }
        public int ProductionOrderId { get; set; }
        [MaxLength(255)]
        public string ProductionOrderNo { get; set; }
        public int FpReturnFromBuyerId { get; set; }
        public virtual FpReturnFromBuyerModel FpReturnFromBuyer { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
