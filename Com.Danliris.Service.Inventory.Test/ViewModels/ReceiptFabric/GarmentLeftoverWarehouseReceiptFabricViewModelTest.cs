using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.ReceiptFabric
{
   public class GarmentLeftoverWarehouseReceiptFabricViewModelTest
    {
        [Fact]
        public void validate_Id()
        {
            //Setup
            GarmentLeftoverWarehouseReceiptFabricViewModel viewModel = new GarmentLeftoverWarehouseReceiptFabricViewModel()
            {
                UENNo = "1"
            };

            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            Mock<IGarmentLeftoverWarehouseReceiptFabricService> GarmentLeftoverWarehouseReceiptFabricMock = new Mock<IGarmentLeftoverWarehouseReceiptFabricService>();
            GarmentLeftoverWarehouseReceiptFabricMock.Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new ReadResponse<GarmentLeftoverWarehouseReceiptFabric>(new List<GarmentLeftoverWarehouseReceiptFabric>(), 1, new Dictionary<string, string>(), new List<string>()));
            serviceProviderMock.Setup(s => s.GetService(typeof(IGarmentLeftoverWarehouseReceiptFabricService))).Returns(GarmentLeftoverWarehouseReceiptFabricMock.Object);

            ValidationContext validationContext = new ValidationContext(viewModel, serviceProviderMock.Object, null);

            //Act
            var result = viewModel.Validate(validationContext);

            //Assert
            Assert.True(0 < result.Count());
        }
    }
}
