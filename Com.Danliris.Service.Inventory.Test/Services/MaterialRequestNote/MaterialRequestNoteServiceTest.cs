using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
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
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Test.Services.MaterialRequestNote
{
    public class MaterialRequestNoteServiceTest
    {
        private const string ENTITY = "MaterialRequestNote";

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

        private MaterialRequestNoteDataUtil _dataUtil(NewMaterialRequestNoteService service)
        {

            GetServiceProvider();
            return new MaterialRequestNoteDataUtil(service);
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
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_CreateAsync()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(data));
        }

        [Fact]
        public async Task Should_Success_DeleteAsync()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(data.Id));
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync_Null()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(utilService).GetEmptyData();
            data.RequestType = "AWAL";

            var model = utilService.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            var model = utilService.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            var model = utilService.Read(1, 25, "{}", null, null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            var model = await utilService.ReadByIdAsync(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Update()
        {
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            var vm = utilService.MapToViewModel(data);
            var testData = utilService.MapToModel(vm);

            testData.MaterialsRequestNote_Items.FirstOrDefault().ProductionOrderId = "3";
            testData.MaterialsRequestNote_Items.Add(new Lib.Models.MaterialsRequestNoteModel.MaterialsRequestNote_Item()
            {
                Grade = "a",
                Length = 1,
                OrderQuantity = 1,
                OrderTypeCode = "code",
                OrderTypeId = "1",
                OrderTypeName = "name",
                ProductCode = "code",
                ProductionOrderNo = "c",
                Remark = "a",
                ProductId = "1",
                ProductionOrderId = "1",
                ProductionOrderIsCompleted = true,
                ProductName = "name"
            });
            var response = await utilService.UpdateAsync(testData.Id, testData);

            Assert.NotEqual(0, response);

            var newData = await utilService.ReadByIdAsync(data.Id);
            var vm2 = utilService.MapToViewModel(newData);
            var testData2 = utilService.MapToModel(vm);
            testData2.MaterialsRequestNote_Items.Clear();
            var newResponse = await utilService.UpdateAsync(testData2.Id, testData2);

            Assert.NotEqual(0, newResponse);
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(data.Id, data));
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync_Null()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(0, new Lib.Models.MaterialsRequestNoteModel.MaterialsRequestNote()));
        }

        [Fact]
        public async Task Should_Success_UpdateIsCompleted()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            await service.UpdateIsCompleted(data.Id, data);
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Success_UpdateIsCompleted_False()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            foreach (var item in data.MaterialsRequestNote_Items)
            {
                item.ProductionOrderIsCompleted = false;
            }
            await service.UpdateIsCompleted(data.Id, data);
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Fail_UpdateIsCompleted()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateIsCompleted(data.Id, data));
        }

        [Fact]
        public async Task Should_Success_UpdateDistributedQuantity()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            service.UpdateDistributedQuantity(data.Id, data);
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Fail_UpdateDistributedQuantity()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();
            Assert.ThrowsAny<Exception>(() => service.UpdateDistributedQuantity(data.Id, data));
        }

        [Fact]
        public async Task Should_Success_GetReport()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var response = service.GetReport(null, null, null, null, null, null, null, 1, 25, "{}", 6);
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Success_GetReport_with_order()
        {
            NewMaterialRequestNoteService service = new NewMaterialRequestNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var orderProperty = new
            {
                Code ="asc"
            };
            string order = JsonConvert.SerializeObject(orderProperty);
            var response = service.GetReport(null, null, null, null, null, null, null, 1, 25, order, 6);
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Success_ValidateVM()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(utilService).GetTestData();

            var vm = utilService.MapToViewModel(data);
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() == 0);


        }

        [Fact]
        public void Should_Success_ValidateNullVM()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new MaterialsRequestNoteViewModel();
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);


        }

        [Fact]
        public void Should_Success_ValidateNullDetailVM()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new MaterialsRequestNoteViewModel()
            {
                RequestType = "a",
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>() {
                    new MaterialsRequestNote_ItemViewModel()
                    {
                         ProductionOrder = new ProductionOrderViewModel()
                        {
                                OrderQuantity = 1
                        }
                    }

                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Validate_When_MaterialsRequestNoteItem_length_isZero()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new MaterialsRequestNoteViewModel()
            {
                RequestType = "a",
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>() {
                    new MaterialsRequestNote_ItemViewModel()
                    {
                        Length =0,

                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Validate_When_Duplicate_ProductionOrderId()
        {
            var serviceProvider = GetServiceProvider();
            NewMaterialRequestNoteService utilService = new NewMaterialRequestNoteService(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var vm = new MaterialsRequestNoteViewModel()
            {
                Id=1,
                RequestType = "a",
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>() {
                    new MaterialsRequestNote_ItemViewModel()
                    {
                        Id =1,
                        Length =2,
                         ProductionOrder = new ProductionOrderViewModel()
                        {
                             Id ="1",
                             OrderQuantity = 1
                        }

                    },
                    new MaterialsRequestNote_ItemViewModel()
                    {
                        Id =1,
                        Length =2,
                         ProductionOrder = new ProductionOrderViewModel()
                        {
                             Id ="1",
                             OrderQuantity = 1
                        }

                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }
    }
}
