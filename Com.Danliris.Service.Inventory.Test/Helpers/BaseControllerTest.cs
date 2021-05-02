using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Helpers
{
    public abstract class BaseControllerTest<TController, TModel, TViewModel, IService>
       where TController : BaseController<TModel, TViewModel, IService>
       where TModel : StandardEntity, IValidatableObject, new()
       where TViewModel : BasicViewModel, IValidatableObject, new()
       where IService : class, IBaseService<TModel, TViewModel>
    {
        protected TModel Model
        {
            get { return new TModel(); }
        }

        protected TViewModel ViewModel
        {
            get { return new TViewModel(); }
        }

        protected List<TModel> Models
        {
            get { return new List<TModel>(); }
        }

        protected List<TViewModel> ViewModels
        {
            get { return new List<TViewModel>(); }
        }

        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IService>());
        }

        protected TController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TController controller = (TController)Activator.CreateInstance(typeof(TController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private int GetStatusCodeGet((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) mocks)
        {
            TController controller = GetController(mocks);
            IActionResult response = controller.Get();

            return GetStatusCode(response);
        }

        [Fact]
        public virtual void Get_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new ReadResponse<TModel>(new List<TModel>() { Model }, 0, new Dictionary<string, string>(), new List<string>()));
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<TModel>())).Returns(ViewModel);
            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public virtual void Get_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<TModel>())).Returns(ViewModel);
            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodePost((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) mocks)
        {
            TController controller = GetController(mocks);
            IActionResult response = await controller.Post(ViewModel);

            return GetStatusCode(response);
        }

        [Fact]
        public virtual async Task Post_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<TViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateAsync(It.IsAny<TModel>())).ReturnsAsync(1);
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public virtual async Task Post_ThrowServiceValidationExeption_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<TViewModel>())).Throws(GetServiceValidationExeption());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public virtual async Task Post_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<TViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateAsync(It.IsAny<TModel>())).ThrowsAsync(new Exception());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodeGetById((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) mocks)
        {
            TController controller = GetController(mocks);
            IActionResult response = await controller.GetById(1);

            return GetStatusCode(response);
        }

        [Fact]
        public virtual async Task GetById_NotNullModel_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<TModel>())).Returns(ViewModel);
            int statusCode = await GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public virtual async Task GetById_NullModel_ReturnNotFound()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync((TModel)null);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<TModel>())).Returns(ViewModel);
            int statusCode = await GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public virtual async Task GetById_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<TModel>())).Returns(ViewModel);
            int statusCode = await GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodePut((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) mocks, int id, TViewModel viewModel)
        {
            TController controller = GetController(mocks);
            IActionResult response = await controller.Put(id, viewModel);

            return GetStatusCode(response);
        }

        [Fact]
        public virtual async Task Put_InvalidId_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<TViewModel>())).Verifiable();
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            var id = 1;
            var viewModel = new TViewModel()
            {
                Id = id + 1
            };

            int statusCode = await GetStatusCodePut(mocks, id, viewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public virtual async Task Put_ValidId_ReturnNoContent()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<TViewModel>())).Verifiable();
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            var id = 1;
            var viewModel = new TViewModel()
            {
                Id = id
            };
            mocks.Service.Setup(f => f.UpdateAsync(It.IsAny<int>(), It.IsAny<TModel>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodePut(mocks, id, viewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public virtual async Task Put_ThrowServiceValidationExeption_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<TViewModel>())).Throws(GetServiceValidationExeption());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePut(mocks, 1, ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public virtual async Task Put_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<TViewModel>())).Verifiable();
            var id = 1;
            var viewModel = new TViewModel()
            {
                Id = id
            };
            mocks.Service.Setup(f => f.UpdateAsync(It.IsAny<int>(), It.IsAny<TModel>())).ThrowsAsync(new Exception());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<TViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePut(mocks, id, viewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodeDelete((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service) mocks)
        {
            TController controller = GetController(mocks);
            IActionResult response = await controller.Delete(1);
            return GetStatusCode(response);
        }

        [Fact]
        public virtual async Task Delete_WithoutException_ReturnNoContent()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteAsync(It.IsAny<int>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodeDelete(mocks);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public virtual async Task Delete_ThrowException_ReturnInternalStatusError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            int statusCode = await GetStatusCodeDelete(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
