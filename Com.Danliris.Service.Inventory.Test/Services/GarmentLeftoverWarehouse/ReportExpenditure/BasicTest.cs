using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFabric;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.ReportExpenditure
{
    public class BasicTest
    {
        const string ENTITY = "GarmentLeftoverWarehouseExpenditureFabric";

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

        [Fact]
        public void GetReportFabric_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object,_dbContext(GetCurrentMethod()));

            var result = service.GetReport(null, DateTime.Now,"FABRIC", 1, 25, "{}" ,1);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetReportAccessories_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            var result = service.GetReport(null, DateTime.Now, "ACCESSORIES", 1, 25, "{}", 1);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetReportAllType_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            var result = service.GetReport(null, DateTime.Now, "", 1, 25, "{}", 1);

            Assert.NotNull(result);
        }

        [Fact]
        public void Excel_ReportFabricSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            var result = service.GenerateExcel(DateTime.Now, DateTime.Now, "FABRIC", 1);

            Assert.IsType<MemoryStream>(result);


        }

        [Fact]
        public void Excel_ReportAccessoriesSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            var result = service.GenerateExcel(DateTime.Now, DateTime.Now, "ACCESSORIES", 1);

            Assert.IsType<MemoryStream>(result);


        }

        [Fact]
        public void Excel_ReportAllTypeSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            var result = service.GenerateExcel(DateTime.Now, DateTime.Now, "", 1);

            Assert.IsType<MemoryStream>(result);


        }
    }
}
