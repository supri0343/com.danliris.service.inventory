using Com.Danliris.Service.Inventory.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels
{
    public class UnitReceiptNoteViewModelTest
    {
        [Fact]
        public void should_succes_instantiate()
        {
            //Act
            UnitReceiptNoteViewModel viewModel = new UnitReceiptNoteViewModel()
            {
                no = "no",
                _id = "1",
                unit = new UnitViewModel(),
                items = new List<UnitReceiptNoteViewModel.Item>(),
                supplier = new SupplierViewModel()
            };

            //Assert
            Assert.Equal("no", viewModel.no);
            Assert.NotNull(viewModel.unit);
            Assert.NotNull(viewModel.items);
            Assert.NotNull(viewModel.supplier);
            Assert.Equal("1", viewModel._id);
        }
    }
}
