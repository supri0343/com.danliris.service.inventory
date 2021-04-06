using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventoryMovementReport
{
    public class InventoryDystuffReportControllerTest
    {
        protected Lib.Models.InventoryModel.InventoryMovement Model
        {
            get { return new Lib.Models.InventoryModel.InventoryMovement(); }
        }
        protected InventoryDocumentViewModel ViewModel
        {
            get { return new InventoryDocumentViewModel(); }
        }

        protected List<Lib.Models.InventoryModel.InventoryMovement> Models
        {
            get { return new List<Lib.Models.InventoryModel.InventoryMovement>(); }
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

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDystuffService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IInventoryDystuffService>());
        }

        protected InventoryDystuffController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryDystuffService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            InventoryDystuffController controller = (InventoryDystuffController)Activator.CreateInstance(typeof(InventoryDystuffController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
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


        [Fact]
        public void Get_Report_Success_Dystuff()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(),  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<InventoryDystuffViewModel>, int>(new List<InventoryDystuffViewModel>(), 1));
            var controller = GetController(mocks);
            var response = controller.GetReportAll(null, null, null, 0, 0);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_Report_Fail_Dystuff()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(),  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.GetReportAll(null, null, null, 1, 25);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetXls_Report_Success_Dystuff()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Returns(new MemoryStream());
            var controller = GetController(mocks);
            var response = controller.GetXlsAll(null, null, null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetXls_Report_Fail_Dystuff()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<string>(),  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.GetXlsAll(null, null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
