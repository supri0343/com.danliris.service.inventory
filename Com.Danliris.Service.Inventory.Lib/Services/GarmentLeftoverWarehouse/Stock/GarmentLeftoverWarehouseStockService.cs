using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockService : IGarmentLeftoverWarehouseStockService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseStockService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseStock> DbSetStock;
        private DbSet<GarmentLeftoverWarehouseStockHistory> DbSetStockHistory;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseStockService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSetStock = DbContext.Set<GarmentLeftoverWarehouseStock>();
            DbSetStockHistory = DbContext.Set<GarmentLeftoverWarehouseStockHistory>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public async Task<int> StockIn(GarmentLeftoverWarehouseStock stock, string StockReferenceNo)
        {
            try
            {
                int Affected = 0;

                var Query = DbSetStock.Where(w => w.ReferenceType == stock.ReferenceType && w.UnitId == stock.UnitId);

                switch (stock.ReferenceType)
                {
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC:
                        Query = Query.Where(w => w.PONo == stock.PONo);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD:
                        Query = Query.Where(w => w.RONo == stock.RONo);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC:
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_ACCECORIES:
                        Query = Query.Where(w => w.ProductId == stock.ProductId && w.UomId == stock.UomId);
                        break;
                }

                var existingStock = Query.SingleOrDefault();
                if (existingStock == null)
                {
                    stock.FlagForCreate(IdentityService.Username, UserAgent);
                    stock.FlagForUpdate(IdentityService.Username, UserAgent);

                    stock.Histories = new List<GarmentLeftoverWarehouseStockHistory>();
                    var stockHistory = new GarmentLeftoverWarehouseStockHistory
                    {
                        StockReferenceNo = StockReferenceNo,
                        StockType = GarmentLeftoverWarehouseStockTypeEnum.IN,
                        BeforeQuantity = 0,
                        Quantity = stock.Quantity,
                        AfterQuantity = stock.Quantity
                    };
                    stockHistory.FlagForCreate(IdentityService.Username, UserAgent);
                    stockHistory.FlagForUpdate(IdentityService.Username, UserAgent);
                    stock.Histories.Add(stockHistory);

                    DbSetStock.Add(stock);
                }
                else
                {
                    existingStock.Quantity += stock.Quantity;
                    existingStock.FlagForUpdate(IdentityService.Username, UserAgent);

                    var lastStockHistory = DbSetStockHistory.Where(w => w.StockId == existingStock.Id).OrderBy(o => o._CreatedUtc).Last();
                    var beforeQuantity = lastStockHistory.AfterQuantity;

                    var stockHistory = new GarmentLeftoverWarehouseStockHistory
                    {
                        StockId = existingStock.Id,
                        StockReferenceNo = StockReferenceNo,
                        StockType = GarmentLeftoverWarehouseStockTypeEnum.IN,
                        BeforeQuantity = beforeQuantity,
                        Quantity = stock.Quantity,
                        AfterQuantity = beforeQuantity + stock.Quantity
                    };
                    stockHistory.FlagForCreate(IdentityService.Username, UserAgent);
                    stockHistory.FlagForUpdate(IdentityService.Username, UserAgent);

                    DbSetStockHistory.Add(stockHistory);
                }

                Affected = await DbContext.SaveChangesAsync();
                return Affected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> StockOut(GarmentLeftoverWarehouseStock stock, string StockReferenceNo)
        {
            try
            {
                int Affected = 0;

                var Query = DbSetStock.Where(w => w.ReferenceType == stock.ReferenceType && w.UnitId == stock.UnitId);

                switch (stock.ReferenceType)
                {
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC:
                        Query = Query.Where(w => w.PONo == stock.PONo);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD:
                        Query = Query.Where(w => w.RONo == stock.RONo);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC:
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_ACCECORIES:
                        Query = Query.Where(w => w.ProductId == stock.ProductId && w.UomId == stock.UomId);
                        break;
                }

                var existingStock = Query.Single();
                existingStock.Quantity -= stock.Quantity;
                existingStock.FlagForUpdate(IdentityService.Username, UserAgent);

                var lastStockHistory = DbSetStockHistory.Where(w => w.StockId == existingStock.Id).OrderBy(o => o._CreatedUtc).Last();
                var beforeQuantity = lastStockHistory.AfterQuantity;

                var stockHistory = new GarmentLeftoverWarehouseStockHistory
                {
                    StockId = existingStock.Id,
                    StockReferenceNo = StockReferenceNo,
                    StockType = GarmentLeftoverWarehouseStockTypeEnum.OUT,
                    BeforeQuantity = beforeQuantity,
                    Quantity = -stock.Quantity,
                    AfterQuantity = beforeQuantity - stock.Quantity
                };
                stockHistory.FlagForCreate(IdentityService.Username, UserAgent);
                stockHistory.FlagForUpdate(IdentityService.Username, UserAgent);

                DbSetStockHistory.Add(stockHistory);

                Affected = await DbContext.SaveChangesAsync();
                return Affected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
