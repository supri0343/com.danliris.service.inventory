using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.GarmentLeftoverWarehouse.Stock
{
  public  class GarmentLeftoverWarehouseStockHistoryViewModelTest
    {
        [Fact]
        public void should_succes_instantiate()
        {
            GarmentLeftoverWarehouseStockHistoryViewModel viewModel = new GarmentLeftoverWarehouseStockHistoryViewModel()
            {
                StockReferenceNo ="1",
                AfterQuantity =1,
                BeforeQuantity =1,
                Quantity =1,
                StockReferenceId =1,
                StockReferenceItemId =1,
                StockType =new Lib.Enums.GarmentLeftoverWarehouseStockTypeEnum(),  
            };

            Assert.Equal("1", viewModel.StockReferenceNo);
            Assert.Equal(1, viewModel.AfterQuantity);
            Assert.Equal(1, viewModel.BeforeQuantity);
            Assert.Equal(1, viewModel.Quantity);
            Assert.Equal(1, viewModel.StockReferenceId);
            Assert.Equal(1, viewModel.StockReferenceItemId);
         
        }
        }
}
