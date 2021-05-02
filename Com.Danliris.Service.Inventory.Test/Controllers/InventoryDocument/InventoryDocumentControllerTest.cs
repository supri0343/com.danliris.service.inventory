using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventoryDocument
{
    public class InventoryDocumentControllerTest
    {
        protected Lib.Models.InventoryModel.InventoryDocument Model
        {
            get { return new Lib.Models.InventoryModel.InventoryDocument(); }
        }

        protected InventoryDocumentViewModel ViewModel
        {
            get { return new InventoryDocumentViewModel(); }
        }

        protected List<Lib.Models.InventoryModel.InventoryDocument> Models
        {
            get { return new List<Lib.Models.InventoryModel.InventoryDocument>(); }
        }

        protected List<InventoryDocumentViewModel> ViewModels
        {
            get { return new List<InventoryDocumentViewModel>(); }
        }

        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDocumentService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IInventoryDocumentService>());
        }

        protected InventoryDocumentController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDocumentService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            InventoryDocumentController controller = (InventoryDocumentController)Activator.CreateInstance(typeof(InventoryDocumentController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
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

        private int GetStatusCodeGet((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDocumentService> Service) mocks)
        {
            InventoryDocumentController controller = GetController(mocks);
            IActionResult response = controller.Get();

            return GetStatusCode(response);
        }

        [Fact]
        public void Get_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks
                .Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<Lib.Models.InventoryModel.InventoryDocument>(new List<Lib.Models.InventoryModel.InventoryDocument>() { new Lib.Models.InventoryModel.InventoryDocument()}, 0, new Dictionary<string, string>(), new List<string>()));
            
            mocks
                .Service
                .Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>()))
                .Returns(ViewModel);

            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>())).Returns(ViewModel);
            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private int GetStatusCodeGetById((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDocumentService> Service) mocks)
        {
            InventoryDocumentController controller = GetController(mocks);
            IActionResult response = controller.GetById(1);

            return GetStatusCode(response);
        }

        [Fact]
        public void GetById_NotNullModel_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadModelById(It.IsAny<int>())).Returns(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>())).Returns(ViewModel);
            int statusCode = GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetById_NullModel_ReturnNotFound()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadModelById(It.IsAny<int>())).Returns((Lib.Models.InventoryModel.InventoryDocument)null);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>())).Returns(ViewModel);
            int statusCode = GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void GetById_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadModelById(It.IsAny<int>())).Throws(new Exception());
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>())).Returns(ViewModel);
            int statusCode = GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodePost((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDocumentService> Service) mocks)
        {
            InventoryDocumentController controller = GetController(mocks);
            IActionResult response = await controller.Post(ViewModel);

            return GetStatusCode(response);
        }

        private async Task<int> GetStatusCodePostMulti((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDocumentService> Service) mocks)
        {
            InventoryDocumentController controller = GetController(mocks);
            IActionResult response = await controller.MultiplePost(new List<InventoryDocumentViewModel>() { ViewModel });

            return GetStatusCode(response);
        }

        [Fact]
        public async Task Post_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryDocumentViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.Create(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>())).ReturnsAsync(1);
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<InventoryDocumentViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task Post_ThrowServiceValidationExeption_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryDocumentViewModel>())).Throws(GetServiceValidationExeption());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<InventoryDocumentViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Post_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryDocumentViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.Create(It.IsAny<Lib.Models.InventoryModel.InventoryDocument>())).ThrowsAsync(new Exception());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<InventoryDocumentViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task PostMulti_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryDocumentViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateMulti(It.IsAny<List<Lib.Models.InventoryModel.InventoryDocument>>())).ReturnsAsync(1);
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<InventoryDocumentViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePostMulti(mocks);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task PostMulti_ThrowServiceValidationExeption_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryDocumentViewModel>())).Throws(GetServiceValidationExeption());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<InventoryDocumentViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePostMulti(mocks);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task PostMulti_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryDocumentViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateMulti(It.IsAny<List<Lib.Models.InventoryModel.InventoryDocument>>())).ThrowsAsync(new Exception());
            mocks.Service.Setup(f => f.MapToModel(It.IsAny<InventoryDocumentViewModel>())).Returns(Model);
            int statusCode = await GetStatusCodePostMulti(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
