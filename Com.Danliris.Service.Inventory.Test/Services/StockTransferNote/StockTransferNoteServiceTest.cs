using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil;
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

namespace Com.Danliris.Service.Inventory.Test.Services.StockTransferNote
{

    public class StockTransferNoteServiceTest
    {
        private const string ENTITY = "StockTransferNote";

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

        private StockTransferNoteDataUtil _dataUtil(NewStockTransferNoteService service)
        {

            GetServiceProvider();
            return new StockTransferNoteDataUtil(service);
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();

            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            NewStockTransferNoteService service = new NewStockTransferNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();

            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync()
        {
            NewStockTransferNoteService service = new NewStockTransferNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));

            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            NewStockTransferNoteService service = new NewStockTransferNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            var vm = service.MapToViewModel(data);

            var model = service.MapToModel(vm);

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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var vm = service.MapToViewModel(data);
            var testData = service.MapToModel(vm);
            testData.StockTransferNoteItems.Add(new Lib.Models.StockTransferNoteModel.StockTransferNote_Item
            {
                ProductCode = "code",
                ProductId = "1",
                ProductName = "name",
                StockQuantity = 2,
                TransferedQuantity = 2,
                UomId = "2",
                UomUnit = "unitA"
            });
            testData.TargetStorageName = "a";

            var response = await service.UpdateAsync(testData.Id, testData);

            Assert.NotEqual(0, response);


            var newData = await service.ReadByIdAsync(data.Id);
            var vm2 = service.MapToViewModel(newData);
            var testData2 = service.MapToModel(vm2);
            testData2.StockTransferNoteItems.Clear();
            var newResponse = await service.UpdateAsync(testData2.Id, testData2);
            Assert.NotEqual(0, newResponse);
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync()
        {
            NewStockTransferNoteService service = new NewStockTransferNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(99, new Lib.Models.StockTransferNoteModel.StockTransferNote()));
        }

        [Fact]
        public async Task Should_Success_ReadModelByNotUser()
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.ReadModelByNotUser(1, 25, "{}", null, null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_UpdateIsApproved()
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();

            var response = await service.UpdateIsApprove(data.Id);

            Assert.True(response);


        }

        [Fact]
        public async Task Should_Fail_UpdateIsApproved()
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();

            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateIsApprove(99));


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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new StockTransferNoteViewModel();
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Should_Success_ValidateNullDetailVM()
        {
            //Setup
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var vm = new StockTransferNoteViewModel()
            {

                StockTransferNoteItems = new List<StockTransferNote_ItemViewModel>()
                {
                    new StockTransferNote_ItemViewModel()
                    {
                        Summary=null
                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);

            //Act
            var response = vm.Validate(validationContext);

            //Assert
            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_when_Duplicate_SummaryId()
        {
            //Setup
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var vm = new StockTransferNoteViewModel()
            {

                StockTransferNoteItems = new List<StockTransferNote_ItemViewModel>() {
                    new StockTransferNote_ItemViewModel()
                    {

                        Summary = new InventorySummaryViewModel()
                        {
                            productId =1,
                            productName ="productName",
                            quantity=1
                        }
                    },

                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);

            //Act
            var response = vm.Validate(validationContext);

            //Assert
            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Validate_When_TransferedQuantity_is_Greater_Than_Stock()
        {
            //Setup
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
            NewStockTransferNoteService service = new NewStockTransferNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var vm = new StockTransferNoteViewModel()
            {

                StockTransferNoteItems = new List<StockTransferNote_ItemViewModel>() {
                    new StockTransferNote_ItemViewModel()
                    {

                        Summary = new InventorySummaryViewModel()
                        {
                            productId =1,
                            productName ="productName",
                            quantity =1
                        },
                        TransferedQuantity=2
                    },
                    new StockTransferNote_ItemViewModel()
                    {

                        Summary = new InventorySummaryViewModel()
                        {
                            productId =1,
                            productName ="productName"
                        },
                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);

            //Act
            var response = vm.Validate(validationContext);

            //Assert
            Assert.True(response.Count() > 0);
        }
    }
}
