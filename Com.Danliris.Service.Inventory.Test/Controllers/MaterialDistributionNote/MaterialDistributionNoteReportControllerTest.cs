using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.MaterialDistributionNote
{
    public class MaterialDistributionNoteReportControllerTest
    {
        protected Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNote Model
        {
            get { return new Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNote(); }
        }

        protected MaterialDistributionNoteViewModel ViewModel
        {
            get { return new MaterialDistributionNoteViewModel(); }
        }

        protected List<Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNote> Models
        {
            get { return new List<Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNote>(); }
        }

        protected List<MaterialDistributionNoteViewModel> ViewModels
        {
            get { return new List<MaterialDistributionNoteViewModel>(); }
        }

        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IMaterialDistributionService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IMaterialDistributionService>());
        }

        protected MaterialDistributionNoteReportController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IMaterialDistributionService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MaterialDistributionNoteReportController controller = (MaterialDistributionNoteReportController)Activator.CreateInstance(typeof(MaterialDistributionNoteReportController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
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
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<MaterialDistributionNoteReportViewModel>, int>(new List<MaterialDistributionNoteReportViewModel>(), 1));
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.Get("1", "1", "1", DateTime.Now, 1, 25);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_WithoutException_ReturnPDF()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetPdfReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new List<MaterialDistributionNoteReportViewModel>()
                {
                    new MaterialDistributionNoteReportViewModel()
                    {
                        ProductName = "name"
                    }
                });
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = controller.Get("1", "1", "1", DateTime.Now, 1, 25);
            Assert.NotNull(response);
        }

        [Fact]
        public void Get_WithException_ReturnPDFBadRequest()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetPdfReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new List<MaterialDistributionNoteReportViewModel>()
                {
                    new MaterialDistributionNoteReportViewModel()
                    {
                        IsDisposition = true
                    }
                });
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = controller.Get("1", "1", "1", DateTime.Now, 1, 25);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Get_WithException_ReturnPDFBadRequestEmpty()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetPdfReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new List<MaterialDistributionNoteReportViewModel>()
                {
                    
                });
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = controller.Get("1", "1", "1", DateTime.Now, 1, 25);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Get_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Throws(new Exception());
            //mocks.Service.Setup(f => f.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>())).Returns(Model);

            var response = GetController(mocks).Get("1", "1", "1", DateTime.Now, 1, 25);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
    }
}
