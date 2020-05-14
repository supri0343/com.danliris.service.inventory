using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodService : IGarmentLeftoverWarehouseExpenditureFinishedGoodService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptFinishedGoodService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseExpenditureFinishedGood> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        public GarmentLeftoverWarehouseExpenditureFinishedGoodService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseExpenditureFinishedGood>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
            
        }
        public async Task<int> CreateAsync(GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.FinishedGoodExpenditureNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);

                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = item.UnitId,
                            UnitCode = item.UnitCode,
                            UnitName = item.UnitName,
                            RONo = item.RONo,
                            Quantity = item.ExpenditureQuantity
                        };
                        await StockService.StockOut(stock, model.FinishedGoodExpenditureNo, model.Id, item.Id);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Created;
        }

        public async Task<int> DeleteAsync(int id)
        {
            int Deleted = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseExpenditureFinishedGood model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = item.UnitId,
                            UnitCode = item.UnitCode,
                            UnitName = item.UnitName,
                            RONo = item.RONo,
                            Quantity = item.ExpenditureQuantity
                        };

                        await StockService.StockIn(stock, model.FinishedGoodExpenditureNo, model.Id, item.Id);
                    }


                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Deleted;
        }

        public GarmentLeftoverWarehouseExpenditureFinishedGood MapToModel(GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel)
        {
            GarmentLeftoverWarehouseExpenditureFinishedGood model = new GarmentLeftoverWarehouseExpenditureFinishedGood();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel, GarmentLeftoverWarehouseExpenditureFinishedGood>.Copy(viewModel, model);

            if (viewModel.Buyer != null)
            {
                model.BuyerId = viewModel.Buyer.Id;
                model.BuyerCode = viewModel.Buyer.Code;
                model.BuyerName = viewModel.Buyer.Name;
            }


            model.Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseExpenditureFinishedGoodItem modelItem = new GarmentLeftoverWarehouseExpenditureFinishedGoodItem();
                PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel, GarmentLeftoverWarehouseExpenditureFinishedGoodItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Unit != null)
                {
                    modelItem.UnitId = long.Parse(viewModelItem.Unit.Id);
                    modelItem.UnitCode = viewModelItem.Unit.Code;
                    modelItem.UnitName = viewModelItem.Unit.Name;
                }

                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel MapToViewModel(GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGood, GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel>.Copy(model, viewModel);

            viewModel.Buyer = new BuyerViewModel
            {
                Id = model.BuyerId,
                Code = model.BuyerCode,
                Name = model.BuyerName
            };

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel viewModelItem = new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodItem, GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>.Copy(modelItem, viewModelItem);


                    viewModelItem.Unit = new UnitViewModel
                    {
                        Id = modelItem.UnitId.ToString(),
                        Code = modelItem.UnitCode,
                        Name = modelItem.UnitName
                    };


                    viewModel.Items.Add(viewModelItem);
                }
            }

            return viewModel;
        }

        public ReadResponse<GarmentLeftoverWarehouseExpenditureFinishedGood> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseExpenditureFinishedGood> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "FinishedGoodExpenditureNo", "BuyerName","BuyerCode", "ExpenditureTo", "OtherDescription"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFinishedGood>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFinishedGood>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFinishedGood>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "FinishedGoodExpenditureNo", "Buyer", "OtherDescription", "ExpenditureTo","ExpenditureDate"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseExpenditureFinishedGood
            {
                Id = s.Id,
                FinishedGoodExpenditureNo=s.FinishedGoodExpenditureNo,
                ExpenditureDate=s.ExpenditureDate,
                ExpenditureTo=s.ExpenditureTo,
                BuyerCode=s.BuyerCode,
                BuyerId=s.BuyerId,
                BuyerName=s.BuyerName,
                OtherDescription=s.OtherDescription
            });

            Pageable<GarmentLeftoverWarehouseExpenditureFinishedGood> pageable = new Pageable<GarmentLeftoverWarehouseExpenditureFinishedGood>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseExpenditureFinishedGood> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseExpenditureFinishedGood>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseExpenditureFinishedGood> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseExpenditureFinishedGood existingModel = await DbSet.AsNoTracking().Include(a=>a.Items).AsNoTracking().Where(w => w.Id == id).AsNoTracking().FirstOrDefaultAsync();
                    


                    foreach (var newItem in model.Items)
                    {
                        //var oldItem = existingModel.Items.Where(x => x.Id == newItem.Id).FirstOrDefault();
                        if (newItem.Id == 0)
                        {
                            newItem.FlagForCreate(IdentityService.Username, UserAgent);
                            newItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                        else
                        {
                            newItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                    }

                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    foreach (var oldItem in existingModel.Items)
                    {
                        GarmentLeftoverWarehouseStock stockIn = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = oldItem.UnitId,
                            UnitCode = oldItem.UnitCode,
                            UnitName = oldItem.UnitName,
                            RONo = oldItem.RONo,
                            Quantity = oldItem.ExpenditureQuantity
                        };

                        await StockService.StockIn(stockIn, existingModel.FinishedGoodExpenditureNo, existingModel.Id, oldItem.Id);
                    }
                    DbSet.Update(model);

                    foreach (var oldItem in existingModel.Items)
                    {
                        var newItem = model.Items.Where(x => x.Id == oldItem.Id).FirstOrDefault();
                        if (newItem == null)
                        {
                            oldItem.FlagForDelete(IdentityService.Username, UserAgent);
                            DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems.Update(oldItem);
                        }
                    }
                    Updated = await DbContext.SaveChangesAsync();

                    foreach (var newItem in model.Items.Where(a=>a._IsDeleted==false))
                    {
                        GarmentLeftoverWarehouseStock stockOut = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = newItem.UnitId,
                            UnitCode = newItem.UnitCode,
                            UnitName = newItem.UnitName,
                            RONo = newItem.RONo,
                            Quantity = newItem.ExpenditureQuantity
                        };
                        await StockService.StockOut(stockOut, model.FinishedGoodExpenditureNo, model.Id, newItem.Id);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Updated;
        }

        private string GenerateNo(GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            string prefix = "BKB" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.FinishedGoodExpenditureNo.StartsWith(prefix))
                .OrderByDescending(o => o.FinishedGoodExpenditureNo)
                .Select(s => int.Parse(s.FinishedGoodExpenditureNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D4")}";

            return curNo;
        }
    }
}
