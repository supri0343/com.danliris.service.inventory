using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
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

        public GarmentLeftoverWarehouseStockViewModel MapToViewModel(GarmentLeftoverWarehouseStock model)
        {
            GarmentLeftoverWarehouseStockViewModel viewModel = new GarmentLeftoverWarehouseStockViewModel();
            PropertyCopier<GarmentLeftoverWarehouseStock, GarmentLeftoverWarehouseStockViewModel>.Copy(model, viewModel);

            viewModel.Unit = new UnitViewModel
            {
                Id = model.UnitId.ToString(),
                Code = model.UnitCode,
                Name = model.UnitName
            };

            viewModel.Product = new ProductViewModel
            {
                Id = model.ProductId.ToString(),
                Code = model.ProductCode,
                Name = model.ProductName
            };

            viewModel.Uom = new UomViewModel
            {
                Id = model.UomId.ToString(),
                Unit = model.UomUnit
            };

            viewModel.LeftoverComodity = new LeftoverComodityViewModel
            {
                Id = model.LeftoverComodityId.GetValueOrDefault(),
                Code = model.ProductCode,
                Name = model.ProductName
            };

            if (model.Histories != null)
            {
                viewModel.Histories = new List<GarmentLeftoverWarehouseStockHistoryViewModel>();
                foreach (var modelHistory in model.Histories)
                {
                    GarmentLeftoverWarehouseStockHistoryViewModel viewModelHistory = new GarmentLeftoverWarehouseStockHistoryViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseStockHistory, GarmentLeftoverWarehouseStockHistoryViewModel>.Copy(modelHistory, viewModelHistory);
                }
            }

            return viewModel;
        }

        public GarmentLeftoverWarehouseStock MapToModel(GarmentLeftoverWarehouseStockViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<GarmentLeftoverWarehouseStock> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseStock> Query = DbSetStock;

            List<string> SearchAttributes = new List<string>()
            {
                 "PONo","RONo", "ProductCode", "ProductName", "UomUnit"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseStock>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseStock>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseStock>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "ReferenceType", "Unit", "PONo","RONo", "Product", "Uom", "Quantity","BasicPrice"
            };

            Pageable<GarmentLeftoverWarehouseStock> pageable = new Pageable<GarmentLeftoverWarehouseStock>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStock> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseStock>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ReadResponse<dynamic> ReadDistinct(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseStock> Query = DbSetStock;

            List<string> SearchAttributes = new List<string>()
            {
                "PONo", "RONo", "ProductCode", "ProductName", "UomUnit"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseStock>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseStock>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseStock>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "PONo"
            };
            var SelectedQuery = Query.Select($"new({string.Join(",", SelectedFields)})");

            SelectedQuery = SelectedQuery.Distinct();

            List<dynamic> Data = SelectedQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToDynamicList();
            int TotalData = SelectedQuery.Count();

            return new ReadResponse<dynamic>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public GarmentLeftoverWarehouseStock ReadById(int Id)
        {
            return DbSetStock.Where(s => s.Id == Id).FirstOrDefault();
        }

        public async Task<int> StockIn(GarmentLeftoverWarehouseStock stock, string StockReferenceNo, int StockReferenceId, int StockReferenceItemId)
        {
            try
            {
                int Affected = 0;

                var Query = DbSetStock.Where(w => w.ReferenceType == stock.ReferenceType && w.UnitId == stock.UnitId);

                switch (stock.ReferenceType)
                {
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC:
                        Query = Query.Where(w => w.PONo == stock.PONo && w.UomId == stock.UomId);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD:
                        Query = Query.Where(w => w.RONo == stock.RONo && w.LeftoverComodityId==stock.LeftoverComodityId && w.UnitId==stock.UnitId);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC:
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG:
                        Query = Query.Where(w => w.ProductId == stock.ProductId && w.UomId == stock.UomId);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT:
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES:
                        Query = Query.Where(w => w.PONo == stock.PONo && w.UomId == stock.UomId && w.ProductId == stock.ProductId);
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
                        StockReferenceId = StockReferenceId,
                        StockReferenceItemId = StockReferenceItemId,
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
                        StockReferenceId = StockReferenceId,
                        StockReferenceItemId = StockReferenceItemId,
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

        public async Task<int> StockOut(GarmentLeftoverWarehouseStock stock, string StockReferenceNo, int StockReferenceId, int StockReferenceItemId)
        {
            try
            {
                int Affected = 0;

                var Query = DbSetStock.Where(w => w.ReferenceType == stock.ReferenceType && w.UnitId == stock.UnitId);

                switch (stock.ReferenceType)
                {
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC:
                        Query = Query.Where(w => w.PONo == stock.PONo && w.UomId == stock.UomId);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD:
                        Query = Query.Where(w => w.RONo == stock.RONo && w.LeftoverComodityId == stock.LeftoverComodityId);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC:
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG:
                        Query = Query.Where(w => w.ProductId == stock.ProductId && w.UomId == stock.UomId);
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT:
                        break;
                    case GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES:
                        Query = Query.Where(w => w.PONo == stock.PONo && w.UomId == stock.UomId && w.ProductId == stock.ProductId);
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
                    StockReferenceId = StockReferenceId,
                    StockReferenceItemId = StockReferenceItemId,
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
