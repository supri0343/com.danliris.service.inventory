using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Facades.Inventory
{
    public class InventorySummaryReportBasicTest
    {
        private const string ENTITY = "InventorySummaries";

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

        private InventorySummaryDataUtil _dataUtil(InventorySummaryService service)
        {

            GetServiceProvider();
            return new InventorySummaryDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        private Mock<IServiceProvider> GetFailServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpFailTestService());



            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });


            return serviceProvider;
        }

        [Fact]
        public async Task Should_Success_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();

            var Response = await service.Create(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.Create(null));
        }

        [Fact]
        public void Should_Success_GenerateExcel()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetTestData();

            var Response = service.GenerateExcel(null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GenerateExcel_with_Empty_Data()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
          
            var Response = service.GenerateExcel(null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReport()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetTestData();

            var Response = service.GetReport(null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReport_with_Order()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetTestData();

            var orderProperty = new
            {
                code = "asc"
            };
            string order = JsonConvert.SerializeObject(orderProperty);

            var Response = service.GetReport(null, null, 1, 25, order, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewDataViewModel();


            var model = service.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.Read(1, 25, "{}", null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadId()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var data2 = _dataUtil(service).GetNewData();

            var Response = await service.Create(data2);
            var models = service.Read(1, 25, "{}", null, "{}");
            var single = models.Data.FirstOrDefault();
            var model = service.ReadModelById(single.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_GetStorageMTR()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var models = service.Read(1, 25, "{}", null, "{}");
            var single = models.Data.FirstOrDefault();
            var model = service.GetByStorageAndMTR(single.StorageName);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_GetInventorySummaries()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var models = service.Read(1, 25, "{}", null, "{}");
            var single = models.Data.FirstOrDefault();
            Dictionary<string, object> prdIds = new Dictionary<string, object>
            {
                { "Id", new List<int>(){ single.ProductId } }
            };
            var model = service.GetInventorySummaries(JsonConvert.SerializeObject(prdIds));

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_GetSummaryByParam()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService service = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var models = service.Read(1, 25, "{}", null, "{}");
            var single = models.Data.FirstOrDefault();
            var model = service.GetSummaryByParams(single.StorageId, single.ProductId, single.UomId);

            Assert.NotNull(model);
        }
    }
}