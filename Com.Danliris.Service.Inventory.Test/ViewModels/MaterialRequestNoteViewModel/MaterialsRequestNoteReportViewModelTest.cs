using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.MaterialRequestNoteViewModel
{
    public class MaterialsRequestNoteReportViewModelTest
    {
        [Fact]
        public void should_succes_instantiate()
        {
            MaterialsRequestNoteReportViewModel viewModel = new MaterialsRequestNoteReportViewModel()
            {
                Code = "Code",
                CreatedDate = DateTime.Now,
                DistributedLength = 1,
                Grade = "A",
                Length = 1,
                OrderNo = "OrderNo",
                OrderQuantity = 1,
                ProductName = "ProductName",
                Remark = "Remark",
                Status = true
            };

            Assert.Equal(1, viewModel.DistributedLength);
            Assert.True(DateTime.MinValue < viewModel.CreatedDate);
            Assert.Equal("A", viewModel.Grade);
            Assert.Equal("Code", viewModel.Code);
            Assert.Equal(1, viewModel.Length);
            Assert.Equal(1, viewModel.OrderQuantity);
            Assert.Equal("OrderNo", viewModel.OrderNo);
            Assert.Equal("ProductName", viewModel.ProductName);
            Assert.Equal("Remark", viewModel.Remark);
            Assert.True(viewModel.Status);

        }
    }
}
