using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.FPReturnInvToPurchasingViewModel
{
  public  class ListFPReturnInvToPurchasingViewModelTest
    {
        [Fact]
        public void should_succes_instantiate()
        {
            ListFPReturnInvToPurchasingViewModel viewModel = new ListFPReturnInvToPurchasingViewModel()
            {
                Id = 1,
                No = "1",
                SupplierName = "SupplierName",
                TotalLength = 1,
                TotalQuantity = 1,
                UnitName = "UnitName",
                _CreatedUtc =DateTime.Now
            };
            Assert.Equal(1, viewModel.Id);
            Assert.Equal("1", viewModel.No);
            Assert.Equal("SupplierName", viewModel.SupplierName);
            Assert.Equal(1, viewModel.TotalLength);
            Assert.Equal(1, viewModel.TotalQuantity);
            Assert.Equal("UnitName", viewModel.UnitName);
            Assert.True(DateTime.MinValue < viewModel._CreatedUtc);
        }
    }
}
