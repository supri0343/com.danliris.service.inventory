using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1;
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
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.MaterialRequestNote
{
    public class MaterialsRequestNoteReportControllerTest
    {
        protected MaterialsRequestNote Model
        {
            get { return new MaterialsRequestNote(); }
        }

        protected MaterialsRequestNoteViewModel ViewModel
        {
            get { return new MaterialsRequestNoteViewModel(); }
        }

        protected List<MaterialsRequestNote> Models
        {
            get { return new List<MaterialsRequestNote>(); }
        }

        protected List<MaterialsRequestNoteViewModel> ViewModels
        {
            get { return new List<MaterialsRequestNoteViewModel>(); }
        }

        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IMaterialRequestNoteService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IMaterialRequestNoteService>());
        }

        protected MaterialsRequestNoteReportController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IMaterialRequestNoteService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MaterialsRequestNoteReportController controller = (MaterialsRequestNoteReportController)Activator.CreateInstance(typeof(MaterialsRequestNoteReportController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
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

        [Fact]
        public void Get_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new Tuple<List<MaterialsRequestNoteReportViewModel>, int>(new List<MaterialsRequestNoteReportViewModel>(), 1));

            mocks.Service.Setup(f => f.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>())).Returns(Model);

            var response = GetController(mocks).Get("a", "a", "1", "1", "1", null, null, 1, 1);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception());

            mocks.Service.Setup(f => f.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>())).Returns(Model);

            var response = GetController(mocks).Get("a", "a", "1", "1", "1", null, null, 1, 1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
    }
}
