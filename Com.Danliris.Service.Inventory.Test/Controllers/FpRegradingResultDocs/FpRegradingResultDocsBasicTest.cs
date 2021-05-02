using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.BasicControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.FpRegradingResultDocs
{

    public class FpRegradingResultDocsBasicTest : BaseControllerTest<FpRegradingResultDocsController, Lib.Models.FpRegradingResultDocs, FpRegradingResultDocsViewModel, IFpRegradingResultDocsService>
    {
        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            FpRegradingResultDocsViewModel vm = new FpRegradingResultDocsViewModel()
            {
                Product = new FpRegradingResultDocsViewModel.product(),
                Machine = new FpRegradingResultDocsViewModel.machine(),
                Supplier = new FpRegradingResultDocsViewModel.supplier(),
                Bon = new FpRegradingResultDocsViewModel.noBon(),
                Details = new List<FpRegradingResultDetailsDocsViewModel>()
                {
                    new FpRegradingResultDetailsDocsViewModel()
                    {
                        Product = new FpRegradingResultDetailsDocsViewModel.product(),
                        LengthBeforeReGrade=1,
                        GradeBefore ="A"
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.FpRegradingResultDocs>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.FpRegradingResultDocs>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
