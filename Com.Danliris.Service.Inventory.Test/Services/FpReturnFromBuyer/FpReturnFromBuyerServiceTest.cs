using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Test.DataUtils.FpReturnFromBuyerDataUtil;
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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.FpReturnFromBuyer
{
    public class FpReturnFromBuyerServiceTest
    {
        private const string ENTITY = "FpReturFromBuyers";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private InventoryDbContext DbContext(string testName)
        {
            DbContextOptionsBuilder<InventoryDbContext> optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            InventoryDbContext dbContext = new InventoryDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private FpReturnFromBuyerDataUtil DataUtil(FpReturnFromBuyerService service)
        {
            return new FpReturnFromBuyerDataUtil(service);
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

        private Mock<IInventoryDocumentService> GetInventoryDocumentService()
        {
            var inventoryDocumentService = new Mock<IInventoryDocumentService>();

            inventoryDocumentService
                .Setup(x => x.Create(It.IsAny<InventoryDocument>()))
                .ReturnsAsync(1);

            return inventoryDocumentService;
        }

        [Fact]
        public async Task Should_Success_CreateAsync()
        {
            FpReturnFromBuyerService service = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = DataUtil(service).GetNewData();
            var response = await service.CreateAsync(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Fail_DeleteAsync()
        {
            FpReturnFromBuyerService service = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(data.Id));
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = DataUtil(utilService).GetEmptyData();

            var model = utilService.MapToModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();
            var model = utilService.MapToViewModel(data);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            await DataUtil(utilService).GetTestData();
            var model = utilService.Read(1, 25, "{}", null, null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();
            var model = await utilService.ReadByIdAsync(data.Id);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Fail_UpdateAsync()
        {
            FpReturnFromBuyerService service = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(data.Id, data));
        }

        [Fact]
        public async Task Should_Success_ValidateVM()
        {
            var serviceProvider = GetServiceProvider();
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();

            var vm = utilService.MapToViewModel(data);
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Should_Success_ValidateNullVM()
        {
            var serviceProvider = GetServiceProvider();
            var vm = new FpReturnFromBuyerViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1)
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Should_Success_ValidateNullDetailVM()
        {
            var serviceProvider = GetServiceProvider();
            var vm = new FpReturnFromBuyerViewModel();
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_DetailExist()
        {
            var serviceProvider = GetServiceProvider();
            var vm = new FpReturnFromBuyerViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1),
                Details =new List<FpReturnFromBuyerDetailViewModel>()
                {
                    new FpReturnFromBuyerDetailViewModel()
                    {
                        Items =new List<FpReturnFromBuyerItemViewModel>()
                        {
                            new FpReturnFromBuyerItemViewModel()
                            {
                                ProductId =0,
                                
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
        public void Should_Success_Validate_ItemProduct_notExist()
        {
            var serviceProvider = GetServiceProvider();
            var vm = new FpReturnFromBuyerViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1),
                Details = new List<FpReturnFromBuyerDetailViewModel>()
                {
                    new FpReturnFromBuyerDetailViewModel()
                    {
                        Items =new List<FpReturnFromBuyerItemViewModel>()
                        {
                            
                        }
                    }
                }
            };
            ValidationContext validationContext = new ValidationContext(vm, serviceProvider.Object, null);
            var response = vm.Validate(validationContext);

            Assert.True(response.Count() > 0);
        }

        [Fact]
        public async Task Should_Success_VoidTransactionById()
        {
            FpReturnFromBuyerService service = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();
            var response = await service.VoidDocumentById(data.Id);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Failed_VoidTransactionById_InvalidId()
        {
            FpReturnFromBuyerService service = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            FpReturnFromBuyerService utilService = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);
            var data = await DataUtil(utilService).GetTestData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.VoidDocumentById(0));
        }


        [Fact]
        public void Should_Succes_Map_To_ViewModel()
        {
            var viewModel = new FpReturnFromBuyerViewModel()
            {
                Buyer = new BuyerViewModel()
                {
                    Code = "BuyerCode",
                    Id = 1,
                    Name = "BuyerName"
                },
                Code = "Code",
                CodeProduct = "CodeProduct",
                CoverLetter = "CoverLetter",
                Date = DateTime.Now,
                Destination = "Destination",
                Details = new List<FpReturnFromBuyerDetailViewModel>()
                {
                    new FpReturnFromBuyerDetailViewModel()
                    {
                        Items = new List<FpReturnFromBuyerItemViewModel>()
                        {
                            new FpReturnFromBuyerItemViewModel()
                            {
                                ColorWay = "ColorWay",
                                DesignCode = "DesignCode",
                                DesignNumber = "DesignNumber",
                                Length = 1,
                                ProductCode = "ProductCode",
                                ProductId = 1,
                                ProductName = "ProductName",
                                Remark = "",
                                ReturnQuantity = 1,
                                UOM = "UOM",
                                UOMId = 1,
                                Weight = 1
                            }
                        },
                        ProductionOrder = new ProductionOrderIntegrationViewModel()
                        {
                            DistributedQuantity = 1,
                            IsCompleted = false,
                            OrderNo = "OrderNo",
                            Id = 1,
                            OrderQuantity = 1,
                            OrderType = new OrderTypeIntegrationViewModel()
                            {
                                Code = "OrderTypeCode",
                                Id = 1,
                                Name = "OrderTypeName"
                            }
                        }
                    }
                },
                SpkNo = "SpkNo",
                Storage = new StorageIntegrationViewModel()
                {
                    code = "StorageCode",
                    name ="StorageName",
                    _id = 1
                }
            };

            FpReturnFromBuyerService service = new FpReturnFromBuyerService(DbContext(GetCurrentMethod()), GetServiceProvider().Object, GetInventoryDocumentService().Object);

            var model = service.MapToModel(viewModel);

            Assert.NotNull(model);
        }
    }
}
