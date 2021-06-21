using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.BalanceStock
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseBalanceStock";

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

        private GarmentLeftoverWarehouseBalanceStockDataUtil _dataUtil(GarmentLeftoverWarehouseBalanceStockService service)
        {
            return new GarmentLeftoverWarehouseBalanceStockDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        [Fact]
        public async Task Read_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            await _dataUtil(service).GetTestData_FINISHEDGOOD();

            var result = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task ReadById_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData_FINISHEDGOOD();

            var result = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_Success_FINISHEDGOOD()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData_FINISHEDGOOD();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Create_Success_FABRIC()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData_FABRIC();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Error()
        {
            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(0, null));
        }

        

        [Fact]
        public async Task Delete_Error()
        {
            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var data = new GarmentLeftoverWarehouseBalanceStockViewModel
            {
                BalanceStockDate = DateTimeOffset.Now,
                TypeOfGoods = "ACCESSORIES",
                Items = new List<GarmentLeftoverWarehouseBalanceStockItemViewModel>
                {
                    new GarmentLeftoverWarehouseBalanceStockItemViewModel
                    {
                        Unit = new UnitViewModel{
                            Id = "1",
                            Code = "Unit",
                            Name = "Unit"
                        },
                        PONo = "PONo",
                        Quantity = 1,
                        Uom = new UomViewModel
                        {
                            Id = "1",
                            Unit = "Uom"
                        },
                        Product= new ProductViewModel
                        {
                            Id="1",
                            Name="name",
                            Code="code"
                        },
                        LeftoverComodity=new LeftoverComodityViewModel
                        {
                            Id=1,
                            Name="name",
                            Code="code"
                        },
                        RONo="ro",
                        Yarn="asd",
                        BasicPrice=1000,
                        Composition="asd",
                        Construction="asda",
                        ProductRemark="aafadf",
                        Width="ada"
                    }
                }
            };

            var result = service.MapToModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseBalanceStockService service = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData_FINISHEDGOOD();

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }
    }
}
