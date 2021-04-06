using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Facades.Inventory
{
    public class InventoryBasicTest
    {
        private const string ENTITY = "InventoryDocument";

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

        private InventoryDocumentDataUtil _dataUtil(InventoryDocumentService service)
        {

            GetServiceProvider();
            return new InventoryDocumentDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test",TimezoneOffset =1 });

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


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();

            var Response = await service.Create(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.Create(null));
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewDataViewModel();


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


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.Read(1, 25, "{}", null, "{}");

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


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.ReadModelById(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_CreateMultiAsync()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();

            var Response = await service.CreateMulti(new List<InventoryDocument>() { data });
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateMultiAsync()
        {
            var serviceProvider = GetServiceProvider();


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateMulti(new List<InventoryDocument>() { new InventoryDocument() }));
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


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();

            var vm = service.MapToViewModel(data);
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() == 0);


        }

        [Fact]
        public void Should_Success_ValidateNullVM()
        {
            var serviceProvider = GetServiceProvider();
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new InventoryDocumentViewModel() { items = new List<InventoryDocumentItemViewModel>() };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Should_Success_Validate_When_Type_ADJ()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            
            serviceProvider
                .Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, dbContext);
            serviceProvider
                .Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, dbContext);
            var vm = new InventoryDocumentViewModel() { 
                no ="1",
                code ="code",
                items = new List<InventoryDocumentItemViewModel>() { 
                    new InventoryDocumentItemViewModel()
                    {
                        quantity =0
                    }
                } 
                ,
                type = "ADJ"
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Should_Success_ValidateNullDetailVM()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());

            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);

            serviceProvider
                .Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, dbContext);
            serviceProvider
                .Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);


            InventoryDocumentService service = new InventoryDocumentService(serviceProvider.Object, dbContext);
            var vm = new InventoryDocumentViewModel()
            {
                items = new List<InventoryDocumentItemViewModel>() {
                    new InventoryDocumentItemViewModel()
                    {
                        quantity =0
                    }
                }
                ,
                type = "Non ADJ"
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

    }
}
