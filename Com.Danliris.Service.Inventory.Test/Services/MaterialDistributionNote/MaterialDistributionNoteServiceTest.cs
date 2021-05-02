using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
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
using Model = Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;

namespace Com.Danliris.Service.Inventory.Test.Services.MaterialDistributionNote
{

    public class MaterialDistributionNoteServiceTest
    {
        private const string ENTITY = "MaterialDistributionNote";

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

        private MaterialDistributionNoteDataUtil _dataUtil(NewMaterialDistributionNoteService service, NewMaterialRequestNoteService mrnService)
        {

            GetServiceProvider();
            return new MaterialDistributionNoteDataUtil(service, mrnService);
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

        private MaterialRequestNoteDataUtil _dataUtilMrn(NewMaterialRequestNoteService service)
        {

            GetServiceProvider();
            return new MaterialRequestNoteDataUtil(service);
        }

        [Fact]
        public async Task Should_Success_CreateAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service, serviceMrn).GetNewData();
            foreach (var item in data.MaterialDistributionNoteItems)
            {
                item.MaterialRequestNoteId = mrn.Id;
                item.MaterialRequestNoteCreatedDateUtc = mrn._CreatedUtc;
                item.MaterialRequestNoteCode = mrn.Code;
            }
            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service, serviceMrn).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(data));
        }

        [Fact]
        public async Task Should_Success_DeleteAsync()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync()
        {
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));

            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        //[Fact]
        //public async Task Should_Success_GetPdfReport()
        //{
        //    var serviceProvider = GetServiceProvider();
        //    Mock<IMaterialRequestNoteService> IMaterialRequestNoteServiceMock = new Mock<IMaterialRequestNoteService>();
        //    IMaterialRequestNoteServiceMock.Setup(s => s.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new MaterialsRequestNote());

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
        //        .Returns(IMaterialRequestNoteServiceMock.Object);

        //    var dbContext = _dbContext(GetCurrentMethod());

        //    NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, dbContext);
        //    NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, dbContext);
        //    var data = await _dataUtil(service, serviceMrn).GetTestData();
        //    var response = service.GetPdfReport(null, null, null, DateTime.UtcNow, 7);
        //    Assert.NotEmpty(response);
        //}

        [Fact]
        public void Should_Success_GetReport()
        {
            var serviceProvider = GetServiceProvider();
            Mock<IMaterialRequestNoteService> IMaterialRequestNoteServiceMock = new Mock<IMaterialRequestNoteService>();
            IMaterialRequestNoteServiceMock.Setup(s => s.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new MaterialsRequestNote());

            serviceProvider
                .Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(IMaterialRequestNoteServiceMock.Object);

            var dbContext = _dbContext(GetCurrentMethod());

            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, dbContext);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service, serviceMrn).GetTestData();
            var response = service.GetReport(null, null, DateTime.UtcNow, 1, 25, "{}", 7);
            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_GetReport_With_Order()
        {
            var serviceProvider = GetServiceProvider();
            Mock<IMaterialRequestNoteService> IMaterialRequestNoteServiceMock = new Mock<IMaterialRequestNoteService>();
            IMaterialRequestNoteServiceMock.Setup(s => s.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new MaterialsRequestNote());

            serviceProvider
                .Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(IMaterialRequestNoteServiceMock.Object);

            var dbContext = _dbContext(GetCurrentMethod());

            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, dbContext);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, dbContext);
            var data = _dataUtil(service, serviceMrn).GetTestData();
            var orderProperty = new
            {
                No = "desc"
            };
            string order = JsonConvert.SerializeObject(orderProperty);
            var response = service.GetReport(null, null, DateTime.UtcNow, 1, 25, order, 7);
            Assert.NotNull(response);
        }

        
        [Fact]
        public void Should_Success_MapToModel()
        {
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service, serviceMrn).GetEmptyData();


            var model = service.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = service.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var model = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Update()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
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
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var vm = service.MapToViewModel(data);
            var testData = service.MapToModel(vm);
            testData.MaterialDistributionNoteItems.Add(new Model.MaterialDistributionNoteItem()
            {
                MaterialDistributionNoteDetails = new List<Model.MaterialDistributionNoteDetail>()
                {
                    new Model.MaterialDistributionNoteDetail()
                }
            });
            testData.UnitName = "a";

            var response = await service.UpdateAsync(testData.Id, testData);

            Assert.NotEqual(0, response);

            var newData3 = await service.ReadByIdAsync(data.Id);
            var vm3 = service.MapToViewModel(newData3);
            var testData3 = service.MapToModel(vm);
            var dItem = testData3.MaterialDistributionNoteItems.FirstOrDefault();
            dItem.MaterialDistributionNoteDetails = new List<Model.MaterialDistributionNoteDetail>()
            {
                new Model.MaterialDistributionNoteDetail()
                {

                }
            };
            var newResponse3 = await service.UpdateAsync(testData3.Id, testData3);
            Assert.NotEqual(0, newResponse3);
            var newData = await service.ReadByIdAsync(data.Id);
            var vm2 = service.MapToViewModel(newData);
            var testData2 = service.MapToModel(vm);
            testData2.MaterialDistributionNoteItems.Clear();
            var newResponse = await service.UpdateAsync(testData2.Id, testData2);
            Assert.NotEqual(0, newResponse);
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync()
        {
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));

            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(99, new Model.MaterialDistributionNote()));
        }

        [Fact]
        public async Task Should_Success_UpdateIsApprove()
        {
            var serviceProvider = GetServiceProvider();
            InventoryDbContext dbContext = _dbContext(GetCurrentMethod());

            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, dbContext);
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            var mrn = await _dataUtilMrn(serviceMrn).GetTestData();
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
            var response = service.UpdateIsApprove(new List<int>() { data.Id });

            Assert.True(response);
        }

        



        [Fact]
        public async Task Should_Success_ValidateVM()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            await inventorySummaryService.Create(new Lib.Models.InventoryModel.InventorySummary()
            {
                StorageName = "Gudang Greige Finishing",
                UomUnit = "MTR",
                ProductCode = "code",
                Quantity = 1
            });
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();

            var vm = service.MapToViewModel(data);
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() == 0);


        }

        [Fact]
        public void Should_Success_ValidateNullVM()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new MaterialDistributionNoteViewModel() { MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>() };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Should_Success_ValidateNullDetailVM()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new MaterialDistributionNoteViewModel() { MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>() { new MaterialDistributionNoteItemViewModel() } };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Validate_MaterialRequestNote_when_ReceivedLength_isNull()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var vm = new MaterialDistributionNoteViewModel()
            {
                Unit =new UnitViewModel()
                {
                    Code = "code",
                    Name = "PRINTING"
                },
                MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>() {
                    new MaterialDistributionNoteItemViewModel()
                    {
                        
                        MaterialRequestNoteCode="MaterialRequestNoteCode",
                        MaterialDistributionNoteDetails= new List<MaterialDistributionNoteDetailViewModel>()
                        {
                           
                           new MaterialDistributionNoteDetailViewModel()
                           {
                               Product   =new ProductViewModel(),
                               ReceivedLength =0
                           }
                        }
                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Validate_MaterialRequestNote_when_Quantity_isNull()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);

            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var vm = new MaterialDistributionNoteViewModel()
            {
                Unit = new UnitViewModel()
                {
                    Code = "code",
                    Name = "PRINTING"
                },
                MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>() {
                    new MaterialDistributionNoteItemViewModel()
                    {
                      
                        MaterialRequestNoteCode="MaterialRequestNoteCode",
                        MaterialDistributionNoteDetails= new List<MaterialDistributionNoteDetailViewModel>()
                        {

                           new MaterialDistributionNoteDetailViewModel()
                           {
                               Product   =new ProductViewModel(),
                               ReceivedLength =1,
                             
                           }
                        }
                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public async Task ValidateVM_ReceivedLength_lessThan_stock()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService serviceMrn = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            InventorySummaryService inventorySummaryService = new InventorySummaryService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventorySummaryService)))
                .Returns(inventorySummaryService);
            await inventorySummaryService.Create(new Lib.Models.InventoryModel.InventorySummary()
            {
                StorageName = "Gudang Greige Finishing",
                UomUnit = "MTR",
                ProductCode = "code",
                Quantity = 0
            });
            InventoryMovementService inventoryMovementService = new InventoryMovementService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            serviceProvider.Setup(s => s.GetService(typeof(IInventoryMovementService)))
                .Returns(inventoryMovementService);

            InventoryDocumentService inventoryDocumentFacade = new InventoryDocumentService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            serviceProvider.Setup(s => s.GetService(typeof(IInventoryDocumentService)))
                .Returns(inventoryDocumentFacade);
            serviceProvider.Setup(x => x.GetService(typeof(IMaterialRequestNoteService)))
                .Returns(serviceMrn);
            //serviceProvider.Setup(s => s.GetService(typeof(InventoryDocumentFacade)))
            //    .Returns(inventoryDocumentFacade);
            NewMaterialDistributionNoteService service = new NewMaterialDistributionNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, serviceMrn).GetTestData();
        
            var vm = service.MapToViewModel(data);
          
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }
    }
}
