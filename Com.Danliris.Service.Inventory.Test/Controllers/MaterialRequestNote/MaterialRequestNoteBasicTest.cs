using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.BasicControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.MaterialRequestNote
{
    public class MaterialRequestNoteBasicTest : BaseControllerTest<MaterialsRequestNoteController, MaterialsRequestNote, MaterialsRequestNoteViewModel, IMaterialRequestNoteService>
    {
        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            MaterialsRequestNoteViewModel vm = new MaterialsRequestNoteViewModel()
            {
                RequestType = "test",
                Unit = new Lib.ViewModels.UnitViewModel()
                {
                    
                }, 
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>()
                {
                    new MaterialsRequestNote_ItemViewModel()
                    {
                        Product = new Lib.ViewModels.ProductViewModel()
                        {
                            Name = "a"
                        },
                        ProductionOrder = new Lib.ViewModels.ProductionOrderViewModel()
                        {
                            OrderNo = "a",
                            OrderType = new Lib.ViewModels.OrderTypeViewModel()
                            {
                                Name = "a"
                            }
                        },
                        Grade = "a",
                        Length = 1,
                        Remark = "a"
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<MaterialsRequestNote>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfNonTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            MaterialsRequestNoteViewModel vm = new MaterialsRequestNoteViewModel()
            {
                RequestType = "aa",
                Unit = new Lib.ViewModels.UnitViewModel()
                {

                },
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>()
                {
                    new MaterialsRequestNote_ItemViewModel()
                    {
                        Product = new Lib.ViewModels.ProductViewModel()
                        {
                            Name = "a"
                        },
                        ProductionOrder = new Lib.ViewModels.ProductionOrderViewModel()
                        {
                            OrderNo = "a",
                            OrderType = new Lib.ViewModels.OrderTypeViewModel()
                            {
                                Name = "a"
                            }
                        },
                        Grade = "a",
                        Length = 1,
                        Remark = "a"
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<MaterialsRequestNote>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());
            
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<MaterialsRequestNote>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError,GetStatusCode(response));
        }

        [Fact]
        public async void PutIsCompleted_WithoutException_ReturnNoContent()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateIsCompleted(It.IsAny<int>(), It.IsAny<MaterialsRequestNote>())).Returns(Task.CompletedTask);

            mocks.Service.Setup(f => f.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>())).Returns(Model);

            var response = await GetController(mocks).PutIsCompleted(1, ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async void PutIsCompleted_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateIsCompleted(It.IsAny<int>(), It.IsAny<MaterialsRequestNote>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>())).Returns(Model);

            var response = await GetController(mocks).PutIsCompleted(1, ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
