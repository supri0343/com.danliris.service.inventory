using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Moonlay.NetCore.Lib.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services
{
  public  class ValidateServiceTest
    {
        [Fact]
        public void Validate_Throws_ServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            StockTransferNoteViewModel viewModel = new StockTransferNoteViewModel();

            ValidateService service = new ValidateService(serviceProvider.Object);
            Assert.Throws<ServiceValidationExeption>(() => service.Validate(viewModel));

        }
    }
}
