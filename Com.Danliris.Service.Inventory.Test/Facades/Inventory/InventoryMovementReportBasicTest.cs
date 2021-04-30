using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Facades.Inventory
{
    public class InventoryMovementReportBasicTest
    {
        private const string ENTITY = "InventoryMovement";

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
            var serviceProvider = new ServiceCollection()
               .AddEntityFrameworkInMemoryDatabase()
               .BuildServiceProvider();

            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            InventoryDbContext dbContext = new InventoryDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private InventoryMovementDataUtil _dataUtil(InventoryMovementService service)
        {

            GetServiceProvider();
            return new InventoryMovementDataUtil(service);
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
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetNewData();

            var Response = await service.Create(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.Create(null));
        }

        [Fact]
        public void Should_Success_GenerateExcel()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetTestData();

            var Response = service.GenerateExcel(null, null, null, null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GenerateExcel_with_EmptyData()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);

            var Response = service.GenerateExcel(null, null, null, null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReport()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetTestData();

            var Response = service.GetReport(null, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReport_With_Order()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetTestData();

            var orderProperty = new
            {
                No = "asc"
            };

            var order = JsonConvert.SerializeObject(orderProperty);

            var Response = service.GetReport(null, null, null, null, null, 1, 25, order, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetNewDataViewModel();


            var model = service.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = await _dataUtil(service).GetTestData();
            var model = service.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = await _dataUtil(service).GetTestData();
            var model = service.Read(1, 25, "{}", null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = await _dataUtil(service).GetTestData();
            var model = service.ReadModelById(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_RefreshMovememnt()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = await _dataUtil(inventoryMovementService).GetTestData();
            var result = await inventoryMovementService.RefreshInventoryMovement();
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Should_Success_GenerateExcel_Dystuff()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetTestData();

            InventoryDystuffService facade = new InventoryDystuffService(serviceProvider.Object, dbContext);
            var Response = facade.GenerateExcel(null, null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GenerateExcel_Dystuff_with_EmptyData()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);

            InventoryDystuffService facade = new InventoryDystuffService(serviceProvider.Object, dbContext);
            var Response = facade.GenerateExcel(null, null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReport_Dystuff()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService service = new InventoryMovementService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service).GetTestData();

            InventoryDystuffService facade = new InventoryDystuffService(serviceProvider.Object, dbContext);
            var Response = facade.GetReport(null, null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response);
        }


    }
}