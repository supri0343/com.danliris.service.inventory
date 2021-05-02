using Com.Danliris.Service.Inventory.Lib.ViewModels.FpRegradingResultDocs;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.FpRegradingResultDocs
{
    public class FpRegradingResultDocsReportViewModelTest
    {

        [Fact]
        public void should_succes_instantiate()
        {
            FpRegradingResultDocsReportViewModel viewModel = new FpRegradingResultDocsReportViewModel()
            {
                Code = "Code",
                IsReturn = true,
                IsReturnedToPurchasing = true,
                ProductName = "ProductName",
                _CreatedUtc = DateTime.Now,
                _LastModifiedUtc = DateTime.Now,
                SupplierName = "SupplierName",
                TotalLength = 1,
                TotalQuantity = 1,
                UnitName = "UnitName",
                
            };
            Assert.True( viewModel.IsReturn);
            Assert.Equal("Code", viewModel.Code);
            Assert.Equal("SupplierName", viewModel.SupplierName);
            Assert.Equal("ProductName", viewModel.ProductName);
            Assert.Equal(1, viewModel.TotalLength);
            Assert.Equal(1, viewModel.TotalQuantity);
            Assert.Equal("UnitName", viewModel.UnitName);
            Assert.True(viewModel.IsReturnedToPurchasing);
            Assert.True(DateTime.MinValue < viewModel._LastModifiedUtc);
            Assert.True(DateTime.MinValue < viewModel._CreatedUtc);
        }
    }
}
