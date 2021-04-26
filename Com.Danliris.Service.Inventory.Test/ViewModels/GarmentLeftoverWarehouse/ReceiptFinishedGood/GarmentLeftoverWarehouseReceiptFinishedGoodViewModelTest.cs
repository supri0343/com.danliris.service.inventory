using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.GarmentLeftoverWarehouse.ReceiptFinishedGood
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodViewModelTest
    {
        [Fact]
        public void validate_Id()
        {
            //Setup
            GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel()
            {
                Items= new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>()
                {
                    new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel
                    {

                        ExpenditureGoodNo = "1"
                    }
                }
            };

            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            Mock<IGarmentLeftoverWarehouseReceiptFinishedGoodService> IGarmentLeftoverWarehouseReceiptFinishedGoodServiceMock = new Mock<IGarmentLeftoverWarehouseReceiptFinishedGoodService>();
            IGarmentLeftoverWarehouseReceiptFinishedGoodServiceMock.Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new ReadResponse<GarmentLeftoverWarehouseReceiptFinishedGood>(new List<GarmentLeftoverWarehouseReceiptFinishedGood>(), 1, new Dictionary<string, string>(), new List<string>()));
            serviceProviderMock.Setup(s => s.GetService(typeof(IGarmentLeftoverWarehouseReceiptFinishedGoodService))).Returns(IGarmentLeftoverWarehouseReceiptFinishedGoodServiceMock.Object);

            ValidationContext validationContext = new ValidationContext(viewModel, serviceProviderMock.Object, null);
            
            //Act
            var result = viewModel.Validate(validationContext);

            //Assert
            Assert.True(0 < result.Count());
        }
    }
}
