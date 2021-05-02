using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel
{
    public class FPReturnInvToPurchasingViewModel : BasicViewModel, IValidatableObject
    {
        public string No { get; set; }
        public UnitViewModel Unit { get; set; }
        public SupplierViewModel Supplier { get; set; }
        public int AutoIncrementNumber { get; set; }
        public List<FPReturnInvToPurchasingDetailViewModel> FPReturnInvToPurchasingDetails { get; set; }

        public FPReturnInvToPurchasingViewModel() { }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int Count = 0;

            if (this.Unit == null || string.IsNullOrWhiteSpace(this.Unit.Name))
                yield return new ValidationResult("Unit is required", new List<string> { "Unit" });

            if (this.Supplier == null || string.IsNullOrWhiteSpace(this.Supplier._id))
                yield return new ValidationResult("Supplier is required", new List<string> { "Supplier" });

            if (FPReturnInvToPurchasingDetails.Count.Equals(0))
            {
                yield return new ValidationResult("Details is required", new List<string> { "FPReturnInvToPurchasingCollection" });
            }
            else
            {
                string DetailError = "[";

                foreach (FPReturnInvToPurchasingDetailViewModel detail in this.FPReturnInvToPurchasingDetails)
                {
                    if (string.IsNullOrWhiteSpace(detail.FPRegradingResultDocsCode))
                    {
                        Count++;
                        DetailError += "{ fpRegradingResultDocs: 'Regrading Result Docs No is required' }, ";
                    }

                    else if (detail.NecessaryLength <= 0)
                    {
                        Count++;
                        DetailError += "{ NecessaryLength: 'Necessary Length must be greater than zero' }, ";
                    }
                }

                if (Count > 0)
                {
                    yield return new ValidationResult(DetailError, new List<string> { "FPReturnInvToPurchasingDetails" });
                }
            }
        }
    }
}
