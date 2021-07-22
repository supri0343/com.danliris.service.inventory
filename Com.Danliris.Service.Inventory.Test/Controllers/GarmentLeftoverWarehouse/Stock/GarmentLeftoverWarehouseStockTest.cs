using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.Stock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockTest
    {
        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseStockService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IGarmentLeftoverWarehouseStockService>());
        }

        protected GarmentLeftoverWarehouseStockController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseStockService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentLeftoverWarehouseStockController controller = new GarmentLeftoverWarehouseStockController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
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

        private int GetStatusCodeGet((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseStockService> Service) mocks)
        {
            GarmentLeftoverWarehouseStockController controller = GetController(mocks);
            IActionResult response = controller.Get();

            return GetStatusCode(response);
        }

        [Fact]
        public virtual void Get_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<GarmentLeftoverWarehouseStock>(new List<GarmentLeftoverWarehouseStock>() { new GarmentLeftoverWarehouseStock() }, 0, new Dictionary<string, string>(), new List<string>()));
            mocks.Service
                .Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseStock>()))
                .Returns(new GarmentLeftoverWarehouseStockViewModel());
            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public virtual void Get_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());
            mocks.Service
                .Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseStock>()))
                .Returns(new GarmentLeftoverWarehouseStockViewModel());
            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private int GetStatusCodeGetDistinct((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseStockService> Service) mocks)
        {
            GarmentLeftoverWarehouseStockController controller = GetController(mocks);
            IActionResult response = controller.GetDistinct();

            return GetStatusCode(response);
        }

        [Fact]
        public virtual void Get_Distinct_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.ReadDistinct(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>() { }, 0, new Dictionary<string, string>(), new List<string>()));
            int statusCode = GetStatusCodeGetDistinct(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public virtual void Get_Distinct_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.ReadDistinct(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());
            int statusCode = GetStatusCodeGetDistinct(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id_NullModel()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(It.IsAny<GarmentLeftoverWarehouseStock>);

            mocks.Service
                .Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseStock>()))
                .Returns(new GarmentLeftoverWarehouseStockViewModel());

            var response = await GetController(mocks).GetById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            GarmentLeftoverWarehouseStock Model = new GarmentLeftoverWarehouseStock
            {
                Id = 1,
                PONo = "po"
            };
            var mocks = GetMocks();
            mocks.Service
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            mocks.Service
                .Setup(f => f.MapToViewModel(Model))
                .Returns(new GarmentLeftoverWarehouseStockViewModel());

            var response = await GetController(mocks).GetById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void Should_Error_Get_Data_By_Id()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Throws(new Exception());

            mocks.Service
                .Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseStock>()))
                .Throws(new Exception());

            var response = await GetController(mocks).GetById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
