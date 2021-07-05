using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.Report
{
    public class ReceiptMonitoringTests
    {
        private const string ENTITY = "GarmentLeftoverWarehouseReceiptMonitoring";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private InventoryDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<InventoryDbContext> optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            InventoryDbContext dbContext = new InventoryDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private GarmentLeftoverWarehouseReceiptFabricDataUtil _dataUtilFabric(GarmentLeftoverWarehouseReceiptFabricService service)
        {
            return new GarmentLeftoverWarehouseReceiptFabricDataUtil(service);
        }

        private GarmentLeftoverWarehouseReceiptAccessoriesDataUtil _dataUtilAcc(GarmentLeftoverWarehouseReceiptAccessoriesService service)
        {
            return new GarmentLeftoverWarehouseReceiptAccessoriesDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        [Fact]
        public async Task Should_Success_GetReport_Fabric()
        {


            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataAvalFabric = await _dataUtilFabric(service).GetTestData(); ;
            var result1 = Reportservice.GetFabricReceiptMonitoring(DateTime.Now, DateTime.Now, 1, 1, "{}", 7);
            Assert.NotNull(result1);


        }

        //[Fact]
        //public async Task Should_Success_GetXlsReport()
        //{
        //    var serviceProvider1 = GetServiceProvider();

        //    var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
        //    stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //        .ReturnsAsync(1);

        //    stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //      .ReturnsAsync(1);

        //    serviceProvider1
        //        .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
        //        .Returns(stockServiceMock.Object);

        //    serviceProvider1
        //        .Setup(x => x.GetService(typeof(IHttpService)))
        //        .Returns(new HttpTestService());

        //    var serviceProvider = new Mock<IServiceProvider>();

        //    var httpClientService = new Mock<IHttpService>();

        //    httpClientService
        //        .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IIdentityService)))
        //        .Returns(new IdentityService() { Token = "Token", Username = "Test" });

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpService)))
        //        .Returns(httpClientService.Object);


        //    GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
        //    ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


        //    var dataAvalFabric = await _dataUtilFabric(service).GetTestData(); ;
        //    var result1 = Reportservice.GenerateExcelFabric(DateTime.Now, DateTime.Now, 7);
        //    Assert.NotNull(result1);

        //}

        //[Fact]
        //public async Task Should_Success_GetXlsReport_zero_data()
        //{


        //    var serviceProvider = GetServiceProvider();

        //    var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
        //    stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //        .ReturnsAsync(1);

        //    stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //      .ReturnsAsync(1);

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
        //        .Returns(stockServiceMock.Object);

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpService)))
        //        .Returns(new HttpTestService());

        //    GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
        //    ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


        //    var result = Reportservice.GenerateExcelFabric(DateTime.Now, DateTime.Now, 7);


        //    Assert.NotNull(result);
        //}
    }
}