using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel
{
    public class FPReturnInvToPurchasingDetailViewModel : BasicViewModel
    {
        public int FPReturnInvToPurchasingId { get; set; }
        public int FPRegradingResultDocsId { get; set; }
        public string FPRegradingResultDocsCode { get; set; }
        public ProductViewModel Product { get; set; }
        public double Quantity { get; set; }
        public double NecessaryLength { get; set; }
        public double Length { get; set; }
        public string Description { get; set; }

        public FPReturnInvToPurchasingDetailViewModel() { }

    }
}
