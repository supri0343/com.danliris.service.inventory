using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.Stock
{
    public class StockOutTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseStockOUT";

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
        public async Task StockOut_PO_Success()
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

            var result = await service.StockIn(stock, "StockReferenceNo", 1, 1);
            await service.StockOut(stock, "StockReferenceNo", 1, 1);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task StockOut_RO_Success()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                RONo = "RONo",
                Quantity = 1
            };

            var result = await service.StockIn(stock, "StockReferenceNo", 1, 1);
            await service.StockOut(stock, "StockReferenceNo", 1, 1);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task StockOut_KG_Success()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                Quantity = 1
            };

            var result = await service.StockIn(stock, "StockReferenceNo", 1, 1);
            await service.StockOut(stock, "StockReferenceNo", 1, 1);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task StockOut_PRODUCT_Success()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_ACCECORIES,
                UnitId = 1,
                UnitCode = "UnitFromCode",
                UnitName = "UnitFromName",
                ProductId = 1,
                UomId = 1,
                Quantity = 1
            };

            var result = await service.StockIn(stock, "StockReferenceNo", 1, 1);
            await service.StockOut(stock, "StockReferenceNo", 1, 1);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task StockOut_Error()
        {
            GarmentLeftoverWarehouseStockService service = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var result = await Assert.ThrowsAnyAsync<Exception>(() => service.StockOut(null, "StockReferenceNo", 1, 1));

            Assert.NotNull(result);
        }
    }
}
