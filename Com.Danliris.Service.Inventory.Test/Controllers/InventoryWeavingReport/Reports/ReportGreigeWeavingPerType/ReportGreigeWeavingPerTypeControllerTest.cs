using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerType;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory.Reports;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventoryWeavingReport.Reports.ReportGreigeWeavingPerType
{
    public class ReportGreigeWeavingPerTypeControllerTest
    {
        protected Lib.Models.InventoryWeavingModel.InventoryWeavingMovement Model
        {
            get { return new Lib.Models.InventoryWeavingModel.InventoryWeavingMovement(); }
        }

        protected ReportGreigeWeavingPerTypeViewModel ViewModel
        {
            get { return new ReportGreigeWeavingPerTypeViewModel(); }
        }


        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IReportGreigeWeavingPerTypeService> service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), service: new Mock<IReportGreigeWeavingPerTypeService>());
        }

        protected ReportGreigeWeavingPerTypeController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IReportGreigeWeavingPerTypeService> service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            ReportGreigeWeavingPerTypeController controller = (ReportGreigeWeavingPerTypeController)Activator.CreateInstance(typeof(ReportGreigeWeavingPerTypeController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.service.Object);
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
        public void Get_Report_Success_GetReport()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GetStockReport(It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new Tuple<List<ReportGreigeWeavingPerTypeViewModel>, int>(new List<ReportGreigeWeavingPerTypeViewModel>(), 1));
            var controller = GetController(mocks);
            var response = controller.GetReport(DateTime.Now, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_Report_Fail_GetReport()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GetStockReport(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.GetReport(DateTime.Now, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetXls_Report_Success_GetReport()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GenerateExcel(It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Returns(new MemoryStream());
            var controller = GetController(mocks);
            var response = controller.GetExcelAll(null, null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetXls_Report_Fail_GetReport()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GenerateExcel(It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.GetExcelAll(null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
