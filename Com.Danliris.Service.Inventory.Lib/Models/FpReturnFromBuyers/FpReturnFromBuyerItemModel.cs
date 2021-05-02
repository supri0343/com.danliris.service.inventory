using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers
{
    public class FpReturnFromBuyerItemModel : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string ColorWay { get; set; }
        [MaxLength(255)]
        public string DesignCode { get; set; }
        [MaxLength(255)]
        public string DesignNumber { get; set; }
        public double Length { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        public int ProductId { get; set; }
        [MaxLength(255)]
        public string ProductName { get; set; }
        [MaxLength(4000)]
        public string Remark { get; set; }
        public double ReturnQuantity { get; set; }
        [MaxLength(255)]
        public string UOM { get; set; }
        public int UOMId { get; set; }
        public double Weight { get; set; }
        public int FpReturnFromBuyerDetailId { get; set; }
        public virtual FpReturnFromBuyerDetailModel FpReturnFromBuyerDetail { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
