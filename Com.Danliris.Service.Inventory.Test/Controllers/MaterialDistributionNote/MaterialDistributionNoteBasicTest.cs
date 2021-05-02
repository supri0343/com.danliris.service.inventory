using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Model = Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.BasicControllers;
using Moq;
using System.Net;

namespace Com.Danliris.Service.Inventory.Test.Controllers.MaterialDistributionNote
{
    
    public class MaterialDistributionNoteBasicTest : BaseControllerTest<MaterialDistributionNoteController, Model.MaterialDistributionNote, MaterialDistributionNoteViewModel, IMaterialDistributionService>
    {
        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            MaterialDistributionNoteViewModel vm = new MaterialDistributionNoteViewModel()
            {
                Unit = new Lib.ViewModels.UnitViewModel()
                {

                },
                MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>()
                {
                    new MaterialDistributionNoteItemViewModel()
                    {
                        MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetailViewModel>()
                        {
                            new MaterialDistributionNoteDetailViewModel()
                            {
                                Product = new Lib.ViewModels.ProductViewModel()
                                {

                                },
                                ProductionOrder = new Lib.ViewModels.ProductionOrderViewModel()
                                {

                                },
                                Supplier = new Lib.ViewModels.SupplierViewModel()
                                {

                                }
                            }
                        }
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Model.MaterialDistributionNote>())).Returns(vm);

            var response = await GetController(mocks).GetPDF(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Model.MaterialDistributionNote>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPDF(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public  void Puts_WithoutException_NoContent()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateIsApprove(It.IsAny<List<int>>())).Returns(true);
            

            var response = GetController(mocks).Puts(new List<int>() { 1});
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Puts_WithoutException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateIsApprove(It.IsAny<List<int>>())).Returns(false);


            var response = GetController(mocks).Puts(new List<int>() { 1 });
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Puts_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateIsApprove(It.IsAny<List<int>>())).Throws(new Exception());


            var response = GetController(mocks).Puts(new List<int>() { 1 });
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
