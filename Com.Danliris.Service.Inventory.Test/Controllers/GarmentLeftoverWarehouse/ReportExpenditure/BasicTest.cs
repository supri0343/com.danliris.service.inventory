using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ReportExpenditure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.ReportExpenditure
{
    public class BasicTest
    {
        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseReportExpenditureService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IGarmentLeftoverWarehouseReportExpenditureService>());
        }

        protected GarmentLeftoverWarehouseReportExpenditureController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseReportExpenditureService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentLeftoverWarehouseReportExpenditureController controller = new GarmentLeftoverWarehouseReportExpenditureController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
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
        public void GetReportFabricTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(),It.IsAny<DateTime>(), "FABRIC", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<int>()));

            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC", It.IsAny<int>(), It.IsAny<int>(),null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportAccessoriesTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<int>()));

            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES", It.IsAny<int>(), It.IsAny<int>(), null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportAllTypeTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<int>()));

            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "", It.IsAny<int>(), It.IsAny<int>(), null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportFabricTest_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC", It.IsAny<int>(), It.IsAny<int>(), null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportAccessoriesTest_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES", It.IsAny<int>(), It.IsAny<int>(), null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportAllTypeTest_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "", It.IsAny<int>(), It.IsAny<int>(), null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetXlsFabricTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC", It.IsAny<int>()));

            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC");
            Assert.NotNull(response);
        }

        [Fact]
        public void GetXlsAccessoriesTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES", It.IsAny<int>()));

            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES");
            Assert.NotNull(response);
        }

        [Fact]
        public void GetXlsAllTypeTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "", It.IsAny<int>()));

            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "");
            Assert.NotNull(response);
        }

        [Fact]
        public void GetXlsFabricTest_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC", It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "FABRIC");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetXlsAccessoriesTest_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES", It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "ACCESSORIES");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetXlsAllTypeTest_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "", It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
