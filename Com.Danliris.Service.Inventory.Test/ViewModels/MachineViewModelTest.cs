using Com.Danliris.Service.Inventory.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels
{
    public class MachineViewModelTest
    {
        [Fact]
        public void should_succes_instantiate()
        {
            MachineViewModel viewModel = new MachineViewModel()
            {
                code = "code",
                _id = "1",
                name = "name"
            };
            Assert.Equal("code", viewModel.code);
            Assert.Equal("name", viewModel.name);
            Assert.Equal("1", viewModel._id);
        }
    }
}
