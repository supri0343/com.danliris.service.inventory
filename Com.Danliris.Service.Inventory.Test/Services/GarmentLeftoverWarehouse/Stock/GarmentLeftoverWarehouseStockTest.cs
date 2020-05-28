using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
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

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseStock";

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
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                PONo = "POSerialNumber",
                Quantity = 1
            };

            await service.StockIn(stock, "StockReferenceNo", 1, 1);
            var result = service.Read(1, 1, "{}", new List<string>(), "", "{}");

            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task ReadById_Success()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                PONo = "POSerialNumber",
                Quantity = 1
            };

            await service.StockIn(stock, "StockReferenceNo", 1, 1);
            var result = service.Read(1, 1, "{}", new List<string>(), "", "{}");

            Assert.NotEmpty(result.Data);

            var id = result.Data.Select(d => d.Id).First();
            var data = service.ReadById(id);

            Assert.NotNull(data);
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            Assert.Throws<NotImplementedException>(() => service.MapToModel(It.IsAny<GarmentLeftoverWarehouseStockViewModel>()));
        }

        [Fact]
        public async Task MapToViewModel()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock data = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                PONo = "POSerialNumber",
                Quantity = 1
            };

            await service.StockIn(data, "StockReferenceNo", 1, 1);

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ReadDistinct_Success()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                PONo = "POSerialNumber",
                Quantity = 1
            };

            await service.StockIn(stock, "StockReferenceNo", 1, 1);
            var result = service.ReadDistinct(1, 1, "{}", new List<string>(), "", "{}");

            Assert.NotEmpty(result.Data);
        }
    }
}
