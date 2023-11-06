using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices
{
    public class GarmentLeftoverWarehouseReceiptAvalService : IGarmentLeftoverWarehouseReceiptAvalService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptAvalService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptAval> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;
        private readonly IGarmentLeftoverWarehouseStockService StockService;
        private readonly string GarmentAvalProductUri;
        private readonly string GarmentAvalComponentUri;
        private readonly ILogHistoryService logHistoryService;
        public GarmentLeftoverWarehouseReceiptAvalService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptAval>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));

            GarmentAvalProductUri = APIEndpoint.GarmentProduction + "aval-products/";
            GarmentAvalComponentUri = APIEndpoint.GarmentProduction + "aval-components/";
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
        }

        public GarmentLeftoverWarehouseReceiptAval MapToModel(GarmentLeftoverWarehouseReceiptAvalViewModel viewModel)
        {
            GarmentLeftoverWarehouseReceiptAval model = new GarmentLeftoverWarehouseReceiptAval();
            PropertyCopier<GarmentLeftoverWarehouseReceiptAvalViewModel, GarmentLeftoverWarehouseReceiptAval>.Copy(viewModel, model);

            if (viewModel.UnitFrom != null)
            {
                model.UnitFromId = long.Parse(viewModel.UnitFrom.Id);
                model.UnitFromCode = viewModel.UnitFrom.Code;
                model.UnitFromName = viewModel.UnitFrom.Name;
            }

            model.Items = new List<GarmentLeftoverWarehouseReceiptAvalItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseReceiptAvalItem modelItem = new GarmentLeftoverWarehouseReceiptAvalItem();
                PropertyCopier<GarmentLeftoverWarehouseReceiptAvalItemViewModel, GarmentLeftoverWarehouseReceiptAvalItem>.Copy(viewModelItem, modelItem);
                
                if (viewModelItem.Product != null)
                {
                    modelItem.ProductId = long.Parse(viewModelItem.Product.Id);
                    modelItem.ProductCode = viewModelItem.Product.Code;
                    modelItem.ProductName = viewModelItem.Product.Name;
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

        public GarmentLeftoverWarehouseReceiptAvalViewModel MapToViewModel(GarmentLeftoverWarehouseReceiptAval model)
        {
            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel = new GarmentLeftoverWarehouseReceiptAvalViewModel();
            PropertyCopier<GarmentLeftoverWarehouseReceiptAval, GarmentLeftoverWarehouseReceiptAvalViewModel>.Copy(model, viewModel);

            viewModel.UnitFrom = new UnitViewModel
            {
                Id = model.UnitFromId.ToString(),
                Code = model.UnitFromCode,
                Name = model.UnitFromName
            };


            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseReceiptAvalItemViewModel viewModelItem = new GarmentLeftoverWarehouseReceiptAvalItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseReceiptAvalItem, GarmentLeftoverWarehouseReceiptAvalItemViewModel>.Copy(modelItem, viewModelItem);

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

        public ReadResponse<GarmentLeftoverWarehouseReceiptAval> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseReceiptAval> Query = DbSet.Include(x => x.Items);

            List<string> SearchAttributes = new List<string>()
            {
                "AvalReceiptNo", "UnitFromName", "AvalType"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptAval>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptAval>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptAval>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "AvalReceiptNo", "UnitFrom", "ReceiptDate","AvalType","TotalAval","Items"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseReceiptAval
            {
                Id = s.Id,
                AvalReceiptNo = s.AvalReceiptNo,
                UnitFromId = s.UnitFromId,
                UnitFromCode = s.UnitFromCode,
                UnitFromName = s.UnitFromName,
                AvalType = s.AvalType,
                TotalAval=s.TotalAval,
                ReceiptDate = s.ReceiptDate,
                Items = s.Items  
            });

            Pageable<GarmentLeftoverWarehouseReceiptAval> pageable = new Pageable<GarmentLeftoverWarehouseReceiptAval>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseReceiptAval> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseReceiptAval>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseReceiptAval> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(GarmentLeftoverWarehouseReceiptAval model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.AvalReceiptNo = GenerateNo(model);
                    List<string> avalItemIds = new List<string>();

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        avalItemIds.Add(item.GarmentAvalProductItemId.ToString());
                    }

                    DbSet.Add(model);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Create Penerimaan Gudang Sisa Aval - " + model.AvalReceiptNo);

                    Created = await DbContext.SaveChangesAsync();

                    if (model.AvalType=="AVAL FABRIC")
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            Quantity = model.TotalAval
                        };
                        await StockService.StockIn(stock, model.AvalReceiptNo, model.Id, 0);


                        await UpdateAvalProductIsReceived(avalItemIds, true);
                    }
                    else if (model.AvalType == "AVAL BAHAN PENOLONG")
                    {
                        foreach (var item in model.Items)
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG,
                                UnitId = model.UnitFromId,
                                UnitCode = model.UnitFromCode,
                                UnitName = model.UnitFromName,
                                Quantity = item.Quantity,
                                ProductCode=item.ProductCode,
                                ProductName=item.ProductName,
                                ProductId=item.ProductId,
                                UomId=item.UomId,
                                UomUnit=item.UomUnit,
                            };
                            await StockService.StockIn(stock, model.AvalReceiptNo, model.Id, item.Id);
                        }
                            
                    }else if (model.AvalType == "AVAL KOMPONEN")
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            Quantity = model.TotalAval
                        };
                        await StockService.StockIn(stock, model.AvalReceiptNo, model.Id, 0);

                        foreach (var item in model.Items)
                        {
                            await UpdateAvalComponentIsReceived(item.AvalComponentId.ToString(), true);
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

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseReceiptAval model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseReceiptAval existingModel = await DbSet.Where(w => w.Id == id).FirstOrDefaultAsync();
                    if (existingModel.ReceiptDate != model.ReceiptDate)
                    {
                        existingModel.ReceiptDate = model.ReceiptDate;
                    }
                    if (existingModel.Remark != model.Remark)
                    {
                        existingModel.Remark = model.Remark;
                    }
                    if(existingModel.TotalAval != model.TotalAval)
                    {
                        existingModel.TotalAval = model.TotalAval;
                    }

                    if(model.AvalType=="AVAL FABRIC")
                    {
                        if (existingModel.TotalAval != model.TotalAval)
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                                UnitId = model.UnitFromId,
                                UnitCode = model.UnitFromCode,
                                UnitName = model.UnitFromName,
                                Quantity = existingModel.TotalAval
                            };
                            await StockService.StockOut(stock, existingModel.AvalReceiptNo, model.Id, 0);

                            GarmentLeftoverWarehouseStock stock1 = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                                UnitId = model.UnitFromId,
                                UnitCode = model.UnitFromCode,
                                UnitName = model.UnitFromName,
                                Quantity = model.TotalAval
                            };
                            await StockService.StockIn(stock1, model.AvalReceiptNo, model.Id, 0);
                            existingModel.TotalAval = model.TotalAval;
                        }
                    }
                    else if(model.AvalType == "AVAL KOMPONEN")
                    {
                        if (existingModel.TotalAval != model.TotalAval)
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                                UnitId = model.UnitFromId,
                                UnitCode = model.UnitFromCode,
                                UnitName = model.UnitFromName,
                                Quantity = existingModel.TotalAval
                            };
                            await StockService.StockOut(stock, existingModel.AvalReceiptNo, model.Id, 0);

                            GarmentLeftoverWarehouseStock stock1 = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                                UnitId = model.UnitFromId,
                                UnitCode = model.UnitFromCode,
                                UnitName = model.UnitFromName,
                                Quantity = model.TotalAval
                            };
                            await StockService.StockIn(stock1, model.AvalReceiptNo, model.Id, 0);
                            existingModel.TotalAval = model.TotalAval;
                        }
                    }
                   

                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Update Penerimaan Gudang Sisa Aval - " + model.AvalReceiptNo);

                    Updated = await DbContext.SaveChangesAsync();

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

        public async Task<int> DeleteAsync(int id)
        {
            int Deleted = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseReceiptAval model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    List<string> avalItemIds = new List<string>();
                    foreach (var item in model.Items)
                    {
                        avalItemIds.Add(item.GarmentAvalProductItemId.ToString());
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Delete Penerimaan Gudang Sisa Aval - " + model.AvalReceiptNo);

                    Deleted = await DbContext.SaveChangesAsync();

                    if(model.AvalType=="AVAL FABRIC")
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            Quantity = model.TotalAval
                        };
                        await StockService.StockOut(stock, model.AvalReceiptNo, model.Id, 0);
                        await UpdateAvalProductIsReceived(avalItemIds, false);
                    }
                    else if (model.AvalType == "AVAL BAHAN PENOLONG")
                    {
                        foreach (var item in model.Items)
                        {
                            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                            {
                                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG,
                                UnitId = model.UnitFromId,
                                UnitCode = model.UnitFromCode,
                                UnitName = model.UnitFromName,
                                Quantity = item.Quantity,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductId = item.ProductId,
                                UomId = item.UomId,
                                UomUnit = item.UomUnit
                            };
                            await StockService.StockOut(stock, model.AvalReceiptNo, model.Id, item.Id);
                        }

                    } else if (model.AvalType == "AVAL KOMPONEN")
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT,
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            Quantity = model.TotalAval
                        };

                        await StockService.StockOut(stock, model.AvalReceiptNo, model.Id, 0);
                        foreach (var item in model.Items)
                        {
                            await UpdateAvalComponentIsReceived(item.AvalComponentId.ToString(), false);
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

        private string GenerateNo(GarmentLeftoverWarehouseReceiptAval model)
        {
            //string code = model.AvalType == "AVAL FABRIC" ? "BMAF" : "BMAC";
            string code = getCode(model.AvalType);
            string prefix = code + model.UnitFromCode.Trim() + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.AvalReceiptNo.StartsWith(prefix))
                .OrderByDescending(o => o.AvalReceiptNo)
                .Select(s => int.Parse(s.AvalReceiptNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D4")}";

            return curNo;
        }

        private string getCode(string avalType)
        {
            string code = "";
            switch (avalType)
            {

                case "AVAL FABRIC":
                    code = "BMAF";
                    break;

                case "AVAL BAHAN PENOLONG":
                    code = "BMAC";
                    break;

                case "AVAL KOMPONEN":
                    code = "BMAK";
                    break;

                default:
                    break;
            }

            return code;
        }
        private async Task UpdateAvalProductIsReceived(List<string> ids, bool IsReceived)
        {

            var stringContentRequest = JsonConvert.SerializeObject(
                new { ids=ids, IsReceived = IsReceived }
            );
            var httpContentRequest = new StringContent(stringContentRequest, Encoding.UTF8, General.JsonMediaType);

            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var response = await httpService.PutAsync(GarmentAvalProductUri + "update-received" , httpContentRequest);
            if (!response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentResponse) ?? new Dictionary<string, object>();

                throw new Exception(string.Concat("Error from '", GarmentAvalProductUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
            }
        }

        private async Task UpdateAvalComponentIsReceived(string id, bool IsReceived)
        {

            var stringContentRequest = JsonConvert.SerializeObject(
                new { id = id, IsReceived = IsReceived }
            );
            var httpContentRequest = new StringContent(stringContentRequest, Encoding.UTF8, General.JsonMediaType);

            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var response = await httpService.PutAsync(GarmentAvalComponentUri + "update-received", httpContentRequest);
            if (!response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentResponse) ?? new Dictionary<string, object>();

                throw new Exception(string.Concat("Error from '", GarmentAvalComponentUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
            }
        }
    }
}
