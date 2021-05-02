using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.FPReturnInvToPurchasing
{
    public class FPReturnInvToPurchasingBasicTest : BaseControllerTest<FPReturnInvToPurchasingController, Lib.Models.FPReturnInvToPurchasingModel.FPReturnInvToPurchasing, FPReturnInvToPurchasingViewModel, IFPReturnInvToPurchasingService>
    {
        public override void Get_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Tuple<List<object>, int, Dictionary<string, string>>(new List<object>(), 1, new Dictionary<string, string>()));
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.FPReturnInvToPurchasingModel.FPReturnInvToPurchasing>())).Returns(ViewModel);
            FPReturnInvToPurchasingController controller = GetController(mocks);
            var response = controller.Get();

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            var vm = new FPReturnInvToPurchasingViewModel()
            {
                Supplier = new Lib.ViewModels.SupplierViewModel(),
                Unit = new Lib.ViewModels.UnitViewModel(),
                FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>()
                {
                    new FPReturnInvToPurchasingDetailViewModel()
                    {
                        Product = new Lib.ViewModels.ProductViewModel()
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.FPReturnInvToPurchasingModel.FPReturnInvToPurchasing>())).Returns(vm);
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = await controller.GetById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void Delete_WithoutException_ReturnNotFound()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteAsync(It.IsAny<int>())).ReturnsAsync(0);
            var vm = new FPReturnInvToPurchasingViewModel()
            {
                Supplier = new Lib.ViewModels.SupplierViewModel(),
                Unit = new Lib.ViewModels.UnitViewModel(),
                FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>()
                {
                    new FPReturnInvToPurchasingDetailViewModel()
                    {
                        Product = new Lib.ViewModels.ProductViewModel()
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.FPReturnInvToPurchasingModel.FPReturnInvToPurchasing>())).Returns(vm);
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }
    }
}
