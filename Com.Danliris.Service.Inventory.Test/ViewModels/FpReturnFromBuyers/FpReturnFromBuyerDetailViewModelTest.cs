using Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.FpReturnFromBuyers
{
    public class FpReturnFromBuyerDetailViewModelTest
    {
        [Fact]
        public void should_succes_instantiate_OrderTypeIntegrationViewModel()
        {
            OrderTypeIntegrationViewModel viewModel = new OrderTypeIntegrationViewModel()
            {
                Code = "Code",
                Name = "Name",
                Id = 1

            };
            Assert.Equal("Name", viewModel.Name);
            Assert.Equal("Code", viewModel.Code);
            Assert.Equal(1, viewModel.Id);


        }

        [Fact]
        public void should_succes_instantiate()
        {

            ProductionOrderIntegrationViewModel viewModel = new ProductionOrderIntegrationViewModel()
            {
                OrderQuantity = 1,
                IsCompleted = true,
                DistributedQuantity = 1,
                OrderType = new OrderTypeIntegrationViewModel()
            };
            Assert.Equal(1, viewModel.OrderQuantity);
            Assert.True(viewModel.IsCompleted);
            Assert.Equal(1, viewModel.DistributedQuantity);
            Assert.NotNull(viewModel.OrderType);
        }


    }
}
