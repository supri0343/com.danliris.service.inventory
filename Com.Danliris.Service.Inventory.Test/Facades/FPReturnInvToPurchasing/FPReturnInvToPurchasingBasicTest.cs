using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.FPReturnInvToPurchasingDataUtil;
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

namespace Com.Danliris.Service.Inventory.Test.Facades.FPReturnInvToPurchasing
{
    public class FPReturnInvToPurchasingBasicTest
    {
        private const string ENTITY = "FPReturnInvToPurchasing";

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

        private FPReturnInvToPurchasingDataUtil _dataUtil(NewFPReturnInvToPurchasingService service, NewFpRegradingResultDocsService fprrService)
        {

            GetServiceProvider();
            return new FPReturnInvToPurchasingDataUtil(service, fprrService);
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

        private FpRegradingResultDataUtil _dataUtilMrn(NewFpRegradingResultDocsService service)
        {

            GetServiceProvider();
            return new FpRegradingResultDataUtil(service);
        }

        [Fact]
        public async Task Should_Success_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service, serviceMrn).GetNewData();
            foreach (var item in data.FPReturnInvToPurchasingDetails)
            {
                item.FPRegradingResultDocsCode = mrn.Code;
                item.FPRegradingResultDocsId = mrn.Id;
            }
            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service, serviceMrn).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(data));
        }

        [Fact]
        public async Task Should_Success_DeleteAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service, serviceMrn).GetEmptyData();


            var model = service.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = service.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadResponse()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadTuple()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = service.Read(1, 25, "{}", null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Update()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var vm = service.MapToViewModel(data);
            var testData = service.MapToModel(vm);
            testData.FPReturnInvToPurchasingDetails.Add(new Lib.Models.FPReturnInvToPurchasingModel.FPReturnInvToPurchasingDetail()
            {

            });
            testData.UnitName = "a";

            var response = await service.UpdateAsync(testData.Id, testData);

            Assert.NotEqual(0, response);

            var newData = await service.ReadByIdAsync(data.Id);
            var vm2 = service.MapToViewModel(newData);
            var testData2 = service.MapToModel(vm);
            testData2.FPReturnInvToPurchasingDetails.Clear();
            var newResponse = await service.UpdateAsync(testData2.Id, testData2);
            Assert.NotEqual(0, newResponse);
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(99, new Lib.Models.FPReturnInvToPurchasingModel.FPReturnInvToPurchasing()));
        }

        [Fact]
        public async Task Should_Success_ValidateVM()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();

            var vm = service.MapToViewModel(data);
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() == 0);


        }

        [Fact]
        public async Task Should_Success_ValidateNullVMAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new FPReturnInvToPurchasingViewModel() { FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>() };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public async Task Should_Success_ValidateNullDetailVMAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new FPReturnInvToPurchasingViewModel() { FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>() { new FPReturnInvToPurchasingDetailViewModel() } };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public async Task Validate_NecessaryLength()
        {
            var serviceProvider = GetServiceProvider();
            NewFpRegradingResultDocsService serviceMrn = new NewFpRegradingResultDocsService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IFpRegradingResultDocsService)))
                .Returns(serviceMrn);
            NewFPReturnInvToPurchasingService service = new NewFPReturnInvToPurchasingService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new FPReturnInvToPurchasingViewModel() { 
                FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>() { 
                    new FPReturnInvToPurchasingDetailViewModel()
                    {
                        FPRegradingResultDocsCode="FPRegradingResultDocsCode",
                        NecessaryLength =0
                    }
                } 
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }
    }
}
