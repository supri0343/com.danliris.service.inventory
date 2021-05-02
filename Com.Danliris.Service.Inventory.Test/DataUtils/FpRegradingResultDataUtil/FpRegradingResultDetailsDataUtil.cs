using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil
{
    public class FpRegradingResultDetailsDataUtil
    {
        private readonly HttpClientTestService client;

        public FpRegradingResultDetailsDataUtil(HttpClientTestService client)
        {
            this.client = client;
        }

        public FpRegradingResultDocsDetails GetNewData()
        {
            ProductViewModel product = ProductDataUtil.GetProduct(client);

            return new FpRegradingResultDocsDetails
            {
                ProductName = product.Name,
                ProductId = product.Id,
                ProductCode = product.Code,
                Quantity = 1,
                Length = 100,
                Grade = "A",
                Remark = "test",
                Retur ="Ya",
                
            };
        }
    }
}
