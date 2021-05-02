using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.FPReturnInvToPurchasingDataUtil
{
    public class FPReturnInvToPurchasingDataUtil
    {
        //private readonly FpRegradingResultDataUtil.FpRegradingResultDataUtil fpRegradingResultDataUtil;
        //private readonly FPReturnInvToPurchasingDetailDataUtil fpReturnInvToPurchasingDetailDataUtil;
        private readonly NewFPReturnInvToPurchasingService Service;
        private readonly NewFpRegradingResultDocsService FPRService;
        public FPReturnInvToPurchasingDataUtil(NewFPReturnInvToPurchasingService service, NewFpRegradingResultDocsService fprService)
        {
            Service = service;
            FPRService = fprService;
        }

        public FPReturnInvToPurchasingViewModel GetEmptyData()
        {
            return new FPReturnInvToPurchasingViewModel
            {
                Unit = new UnitViewModel(),
                Supplier = new SupplierViewModel(),
                FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>() { new FPReturnInvToPurchasingDetailViewModel() {
                    Product = new ProductViewModel()
                } }
            };
        }

        public FPReturnInvToPurchasing GetNewData()
        {
            
            FPReturnInvToPurchasing TestData = new FPReturnInvToPurchasing
            {
                UnitName = "unit",
                SupplierId = "1",
                SupplierName = "name",
                SupplierCode = "code",
                FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetail>()
                {
                    new FPReturnInvToPurchasingDetail()
                    {
                        FPRegradingResultDocsId = 1,
                        FPRegradingResultDocsCode = "code",
                        ProductId = "1",
                        ProductCode = "coode",
                        ProductName = "name",
                        Quantity = 1,
                        NecessaryLength = 1,
                        Length = 1,
                        Description = ""
                    }
                }
            };

            return TestData;
        }


        public async Task<FPReturnInvToPurchasing> GetTestData()
        {
            FpRegradingResultDataUtil.FpRegradingResultDataUtil fprsDataUtil = new FpRegradingResultDataUtil.FpRegradingResultDataUtil(FPRService);
            var regradDocs = await fprsDataUtil.GetTestData();
            FPReturnInvToPurchasing Data = GetNewData();
            foreach(var item in Data.FPReturnInvToPurchasingDetails)
            {
                item.FPRegradingResultDocsId = regradDocs.Id;
                item.FPRegradingResultDocsCode = regradDocs.Code;
            }
            await this.Service.CreateAsync(Data);
            return Data;
        }
    }
}
