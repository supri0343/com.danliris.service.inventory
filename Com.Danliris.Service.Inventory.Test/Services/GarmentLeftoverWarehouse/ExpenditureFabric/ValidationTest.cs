using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class ValidationTest
    {
        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            return serviceProvider;
        }

        [Fact]
        public void Validate_Null()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel();
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void Validate_ExpenditureDate_MinValue()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
            {
                ExpenditureDate = DateTimeOffset.MinValue
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void Validate_ExpenditureDate_MoreThanToday()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
            {
                ExpenditureDate = DateTimeOffset.Now.AddDays(4)
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void ValidateViewModel_UNIT()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
            {
                ExpenditureDestination = "UNIT",
                UnitExpenditure = null
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void ValidateViewModel_JUAL_LOKAL()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
            {
                ExpenditureDestination = "JUAL LOKAL",
                Buyer = null
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void ValidateViewModel_LAIN_LAIN()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
            {
                ExpenditureDestination = "LAIN-LAIN",
                EtcRemark = null
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void Validate_Items_Empty()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel
            {
                Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>()
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void Validate_Items_Null()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel
            {
                Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel()
                }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void Validate_Items_StockId_Duplicate()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel
            {
                Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
                        PONo = "PONo",
                        Uom = new UomViewModel(),
                        StockId = 1
                    },
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
                        PONo = "PONo",
                        Uom = new UomViewModel(),
                        StockId = 1
                    },
                }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void Validate_Items_Quantity_Over()
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel
            {
                Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
                        StockId = 1,
                        Quantity = 2
                    }
                }
            };

            var mockService = new Mock<IGarmentLeftoverWarehouseExpenditureFabricService>();
            mockService.Setup(s => s.CheckStockQuantity(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);

            var mockSerivceProvider = GetServiceProvider();
            mockSerivceProvider.Setup(s => s.GetService(typeof(IGarmentLeftoverWarehouseExpenditureFabricService)))
                .Returns(mockService.Object);

            var validationContext = new ValidationContext(viewModel, mockSerivceProvider.Object, null);

            var result = viewModel.Validate(validationContext);
            Assert.True(result.Count() > 0);
        }
    }
}
