using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStock : StandardEntity, IValidatableObject
    {
        public DateTimeOffset BalanceStockDate { get; set; }
        public string TypeOfGoods { get; set; }
        public virtual ICollection<GarmentLeftoverWarehouseBalanceStockItem> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
