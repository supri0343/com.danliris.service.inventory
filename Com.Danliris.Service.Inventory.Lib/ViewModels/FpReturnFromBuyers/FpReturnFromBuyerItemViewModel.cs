using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers
{
    public class FpReturnFromBuyerItemViewModel : BasicViewModel, IValidatableObject
    {
        public string ColorWay { get; set; }
        public string DesignCode { get; set; }
        public string DesignNumber { get; set; }
        public double Length { get; set; }
        public string ProductCode { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Remark { get; set; }
        public double ReturnQuantity { get; set; }
        public string UOM { get; set; }
        public int UOMId { get; set; }
        public double Weight { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
