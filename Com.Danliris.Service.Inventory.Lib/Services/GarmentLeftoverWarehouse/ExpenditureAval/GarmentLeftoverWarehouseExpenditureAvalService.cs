using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAvalService : IGarmentLeftoverWarehouseExpenditureAvalService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptAvalService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseExpenditureAval> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;
        private readonly ILogHistoryService logHistoryService;
        public GarmentLeftoverWarehouseExpenditureAvalService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseExpenditureAval>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
        }
        public async Task<int> CreateAsync(GarmentLeftoverWarehouseExpenditureAval model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.AvalExpenditureNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        if (model.AvalType == "AVAL FABRIC" || model.AvalType == "AVAL KOMPONEN")
                        {
                            var receiptAval = DbContext.GarmentLeftoverWarehouseReceiptAvals.Where(a => a.Id == item.AvalReceiptId).Single();
                            receiptAval.IsUsed = true;
                        }
                            
                        
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Create Pengeluaran Gudang Sisa Aval - " + model.AvalExpenditureNo);

                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        if(model.AvalType=="AVAL FABRIC" )
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity
                            };
                            await StockService.StockOut(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
                        else if(model.AvalType == "AVAL KOMPONEN")
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity
                            };
                            await StockService.StockOut(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
                        else
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductId = item.ProductId,
                                UomId = item.UomId,
                                UomUnit = item.UomUnit,
                            };
                            await StockService.StockOut(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
                        
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
                    GarmentLeftoverWarehouseExpenditureAval model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        if (model.AvalType == "AVAL FABRIC" || model.AvalType == "AVAL KOMPONEN")
                        {
                            var receiptAval = DbContext.GarmentLeftoverWarehouseReceiptAvals.Where(a => a.Id == item.AvalReceiptId).Single();
                            receiptAval.IsUsed = false;
                        }
                            
                        
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Delete Pengeluaran Gudang Sisa Aval - " + model.AvalExpenditureNo);

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        if (model.AvalType == "AVAL FABRIC" )
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity
                            };

                            await StockService.StockIn(stock, model.AvalExpenditureNo, model.Id, item.Id);

                        }
                        else if(model.AvalType == "AVAL KOMPONEN")
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity
                            };

                            await StockService.StockIn(stock, model.AvalExpenditureNo, model.Id, item.Id);

                        }
                        else
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductId = item.ProductId,
                                UomId = item.UomId,
                                UomUnit = item.UomUnit,
                            };
                            await StockService.StockIn(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
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

        public GarmentLeftoverWarehouseExpenditureAval MapToModel(GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel)
        {
            GarmentLeftoverWarehouseExpenditureAval model = new GarmentLeftoverWarehouseExpenditureAval();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureAvalViewModel, GarmentLeftoverWarehouseExpenditureAval>.Copy(viewModel, model);

            if (viewModel.Buyer != null)
            {
                model.BuyerId = viewModel.Buyer.Id;
                model.BuyerCode = viewModel.Buyer.Code;
                model.BuyerName = viewModel.Buyer.Name;
            }


            model.Items = new List<GarmentLeftoverWarehouseExpenditureAvalItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseExpenditureAvalItem modelItem = new GarmentLeftoverWarehouseExpenditureAvalItem();
                PropertyCopier<GarmentLeftoverWarehouseExpenditureAvalItemViewModel, GarmentLeftoverWarehouseExpenditureAvalItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Unit != null)
                {
                    modelItem.UnitId = long.Parse(viewModelItem.Unit.Id);
                    modelItem.UnitCode = viewModelItem.Unit.Code;
                    modelItem.UnitName = viewModelItem.Unit.Name;
                }

                if (viewModelItem.Product != null)
                {
                    modelItem.ProductId = long.Parse(viewModelItem.Product.Id);
                    modelItem.ProductName = viewModelItem.Product.Name;
                    modelItem.ProductCode = viewModelItem.Product.Code;
                }

                if (viewModelItem.Uom != null)
                {
                    modelItem.UomId = long.Parse(viewModelItem.Uom.Id);
                    modelItem.UomUnit = viewModelItem.Uom.Unit;
                }
                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseExpenditureAvalViewModel MapToViewModel(GarmentLeftoverWarehouseExpenditureAval model)
        {
            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel = new GarmentLeftoverWarehouseExpenditureAvalViewModel();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureAval, GarmentLeftoverWarehouseExpenditureAvalViewModel>.Copy(model, viewModel);

            viewModel.Buyer = new BuyerViewModel
            {
                Id = model.BuyerId,
                Code = model.BuyerCode,
                Name = model.BuyerName
            };

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseExpenditureAvalItemViewModel viewModelItem = new GarmentLeftoverWarehouseExpenditureAvalItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseExpenditureAvalItem, GarmentLeftoverWarehouseExpenditureAvalItemViewModel>.Copy(modelItem, viewModelItem);


                    viewModelItem.Unit = new UnitViewModel
                    {
                        Id = modelItem.UnitId.ToString(),
                        Code = modelItem.UnitCode,
                        Name = modelItem.UnitName
                    };

                    viewModelItem.Product = new ProductViewModel
                    {
                        Id = modelItem.ProductId.ToString(),
                        Code = modelItem.ProductCode,
                        Name = modelItem.ProductName
                    };

                    viewModelItem.Uom = new UomViewModel
                    {
                        Id = modelItem.UomId.ToString(),
                        Unit = modelItem.UomUnit
                    };

                    viewModel.Items.Add(viewModelItem);
                }
            }

            return viewModel;
        }

        public ReadResponse<GarmentLeftoverWarehouseExpenditureAval> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseExpenditureAval> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "AvalExpenditureNo", "BuyerName","BuyerCode", "ExpenditureTo", "OtherDescription"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureAval>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureAval>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureAval>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "AvalExpenditureNo", "Buyer", "AvalType", "OtherDescription", "ExpenditureTo","ExpenditureDate"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseExpenditureAval
            {
                Id = s.Id,
                AvalExpenditureNo = s.AvalExpenditureNo,
                ExpenditureDate = s.ExpenditureDate,
                ExpenditureTo = s.ExpenditureTo,
                BuyerCode = s.BuyerCode,
                BuyerId = s.BuyerId,
                BuyerName = s.BuyerName,
                OtherDescription = s.OtherDescription,
                AvalType=s.AvalType
            });

            Pageable<GarmentLeftoverWarehouseExpenditureAval> pageable = new Pageable<GarmentLeftoverWarehouseExpenditureAval>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseExpenditureAval> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseExpenditureAval>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseExpenditureAval> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseExpenditureAval model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {

                    int Updated = 0;

                    GarmentLeftoverWarehouseExpenditureAval existingModel = await ReadByIdAsync(id);
                    if (existingModel.ExpenditureDate != model.ExpenditureDate)
                    {
                        existingModel.ExpenditureDate = model.ExpenditureDate;
                    }
                    if (existingModel.Description != model.Description)
                    {
                        existingModel.Description = model.Description;
                    }
                    
                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    foreach (var existingItem in existingModel.Items)
                    {
                        if(existingModel.AvalType=="AVAL FABRIC")
                        {
                            GarmentLeftoverWarehouseStock stockIn = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                                UnitId = existingItem.UnitId,
                                UnitCode = existingItem.UnitCode,
                                UnitName = existingItem.UnitName,
                                Quantity = existingItem.Quantity
                            };
                            await StockService.StockIn(stockIn, model.AvalExpenditureNo, model.Id, existingItem.Id);
                        }
                        else if(existingModel.AvalType == "AVAL KOMPONEN")
                        {
                            GarmentLeftoverWarehouseStock stockIn = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                                UnitId = existingItem.UnitId,
                                UnitCode = existingItem.UnitCode,
                                UnitName = existingItem.UnitName,
                                Quantity = existingItem.Quantity
                            };
                            await StockService.StockIn(stockIn, model.AvalExpenditureNo, model.Id, existingItem.Id);
                        }
                        else
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG,
                                UnitId = existingItem.UnitId,
                                UnitCode = existingItem.UnitCode,
                                UnitName = existingItem.UnitName,
                                Quantity = existingItem.Quantity,
                                ProductCode = existingItem.ProductCode,
                                ProductName = existingItem.ProductName,
                                ProductId = existingItem.ProductId,
                                UomId = existingItem.UomId,
                                UomUnit = existingItem.UomUnit,
                            };
                            await StockService.StockIn(stock, model.AvalExpenditureNo, model.Id, existingItem.Id);
                        }
                        

                        
                    }

                    foreach (var existingItem in existingModel.Items)
                    {
                        var item = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                        if (item == null)
                        {
                            if (model.AvalType == "AVAL FABRIC" || model.AvalType == "AVAL KOMPONEN")
                            {
                                var receiptAval = DbContext.GarmentLeftoverWarehouseReceiptAvals.Where(a => a.Id == existingItem.AvalReceiptId).Single();
                                receiptAval.IsUsed = false;
                            }

                            existingItem.FlagForDelete(IdentityService.Username, UserAgent);
                        }
                        else
                        {
                            if (existingItem.Quantity != item.Quantity)
                            {
                                existingItem.Quantity = item.Quantity;
                            }
                            if (existingItem.ActualQuantity != item.ActualQuantity)
                            {
                                existingItem.ActualQuantity = item.ActualQuantity;
                            }
                            existingItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                    }

                    foreach (var item in model.Items.Where(i => i.Id == 0))
                    {
                        if (model.AvalType == "AVAL FABRIC" || model.AvalType == "AVAL KOMPONEN")
                        {
                            var receiptAval = DbContext.GarmentLeftoverWarehouseReceiptAvals.Where(a => a.Id == item.AvalReceiptId).Single();
                            receiptAval.IsUsed = true;
                        }
                            
                        
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        existingModel.Items.Add(item);
                    }

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Update Pengeluaran Gudang Sisa Aval - " + model.AvalExpenditureNo);

                    Updated = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        if(model.AvalType=="AVAL FABRIC")
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity
                            };

                            await StockService.StockOut(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
                        else if (model.AvalType == "AVAL KOMPONEN")
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity
                            };

                            await StockService.StockOut(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
                        else
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG,
                                UnitId = item.UnitId,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                Quantity = item.Quantity,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductId = item.ProductId,
                                UomId = item.UomId,
                                UomUnit = item.UomUnit,
                            };
                            await StockService.StockOut(stock, model.AvalExpenditureNo, model.Id, item.Id);
                        }
                        
                    }

                    transaction.Commit();

                    return Updated;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        private string GenerateNo(GarmentLeftoverWarehouseExpenditureAval model)
        {
            string code = model.AvalType == "AVAL FABRIC" ? "BKAF" : model.AvalType == "AVAL KOMPONEN" ?  "BKAK":"BKAC";
            string prefix = code + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.AvalExpenditureNo.StartsWith(prefix))
                .OrderByDescending(o => o.AvalExpenditureNo)
                .Select(s => int.Parse(s.AvalExpenditureNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D5")}";

            return curNo;
        }
    }
}
