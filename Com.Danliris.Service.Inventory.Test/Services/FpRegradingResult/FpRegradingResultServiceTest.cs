using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.FpRegradingResult
{

    public class FpRegradingResultServiceTest
    {
        private const string ENTITY = "FpRegradingResult";

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

        private FpRegradingResultDataUtil _dataUtil(NewFpRegradingResultDocsService service)
        {

            GetServiceProvider();
            return new FpRegradingResultDataUtil(service);
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
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();

            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(data));
        }

        [Fact]
        public async Task Should_Success_DeleteAsync()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();

            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync()
        {
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));

            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetEmptyData();


            var model = service.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Update()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var vm = service.MapToViewModel(data);
            var testData = service.MapToModel(vm);
            testData.Details.Add(new FpRegradingResultDocsDetails
            {
                Grade = "a",
                Length = 12,
                ProductCode = "code",
                ProductId = "1",
                ProductName = "name",
                Quantity = 1,
                Remark = "remar",
                Retur = "retur"
            });
            testData.UnitName = "a";

            var response = await service.UpdateAsync(testData.Id, testData);

            Assert.NotEqual(0, response);


            var newData = await service.ReadByIdAsync(data.Id);
            var vm2 = service.MapToViewModel(newData);
            var testData2 = service.MapToModel(vm2);
            testData2.Details.Clear();
            var newResponse = await service.UpdateAsync(testData2.Id, testData2);
            Assert.NotEqual(0, newResponse);
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync()
        {
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(99, new Lib.Models.FpRegradingResultDocs()));
        }

        [Fact]
        public async Task Should_Success_ReadNo()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.ReadNo();

            Assert.NotNull(model);
        }

        [Fact]
        public async Task CustomCodeGenerator()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);


            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, dbContext);
            var data = await _dataUtil(service).GetTestData();
            var result = service.CustomCodeGenerator(data);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_UpdateIsReturnedToPurchasing()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);

            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var response = service.UpdateIsReturnedToPurchasing(data.Id, true);

            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Success_GetReport()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.GetReport(null, null, null, null, null, null, null, 1, 25, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_GetReport_with_order()
        {
            //Setup
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext =  _dbContext(GetCurrentMethod());
            
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            
            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, dbContext);
            var data = await _dataUtil(service).GetTestData();
            var orderProperty = new
            {
                Code ="desc"
            };
            string order = JsonConvert.SerializeObject(orderProperty);

            //Act
            var model = service.GetReport(null, null, null, null, null, null, null, 1, 25, order);

            //Assert
            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ValidateVM()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);

            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();

            var vm = service.MapToViewModel(data);
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() == 0);


        }

        [Fact]
        public async Task Should_Success_ValidateNullVMAsync()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);

            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new FpRegradingResultDocsViewModel() { Details = new List<FpRegradingResultDetailsDocsViewModel>() };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public async Task Should_Success_ValidateNullDetailVMAsync()
        {
            var serviceProvider = GetServiceProvider();

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);

            NewFpRegradingResultDocsService service = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new FpRegradingResultDocsViewModel() { Details = new List<FpRegradingResultDetailsDocsViewModel>() { new FpRegradingResultDetailsDocsViewModel() } };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }
    }
}
