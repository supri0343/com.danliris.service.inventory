using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices
{
    public class GarmentLeftoverWarehouseReceiptFabricService : IGarmentLeftoverWarehouseReceiptFabricService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptFabricService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptFabric> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        private readonly string GarmentUnitReceiptNoteUri;

        private readonly ILogHistoryService logHistoryService;

        public GarmentLeftoverWarehouseReceiptFabricService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptFabric>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
            GarmentUnitReceiptNoteUri = APIEndpoint.Purchasing + "garment-unit-expenditure-notes/";
        }

        public GarmentLeftoverWarehouseReceiptFabric MapToModel(GarmentLeftoverWarehouseReceiptFabricViewModel viewModel)
        {
            GarmentLeftoverWarehouseReceiptFabric model = new GarmentLeftoverWarehouseReceiptFabric();
            PropertyCopier<GarmentLeftoverWarehouseReceiptFabricViewModel, GarmentLeftoverWarehouseReceiptFabric>.Copy(viewModel, model);

            if (viewModel.UnitFrom != null)
            {
                model.UnitFromId = long.Parse(viewModel.UnitFrom.Id);
                model.UnitFromCode = viewModel.UnitFrom.Code;
                model.UnitFromName = viewModel.UnitFrom.Name;
            }

            if (viewModel.StorageFrom != null)
            {
                model.StorageFromId = long.Parse(viewModel.StorageFrom._id);
                model.StorageFromCode = viewModel.StorageFrom.code;
                model.StorageFromName = viewModel.StorageFrom.name;
            }

            model.Items = new List<GarmentLeftoverWarehouseReceiptFabricItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseReceiptFabricItem modelItem = new GarmentLeftoverWarehouseReceiptFabricItem();
                PropertyCopier<GarmentLeftoverWarehouseReceiptFabricItemViewModel, GarmentLeftoverWarehouseReceiptFabricItem>.Copy(viewModelItem, modelItem);

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

        public GarmentLeftoverWarehouseReceiptFabricViewModel MapToViewModel(GarmentLeftoverWarehouseReceiptFabric model)
        {
            GarmentLeftoverWarehouseReceiptFabricViewModel viewModel = new GarmentLeftoverWarehouseReceiptFabricViewModel();
            PropertyCopier<GarmentLeftoverWarehouseReceiptFabric, GarmentLeftoverWarehouseReceiptFabricViewModel>.Copy(model, viewModel);

            viewModel.UnitFrom = new UnitViewModel
            {
                Id = model.UnitFromId.ToString(),
                Code = model.UnitFromCode,
                Name = model.UnitFromName
            };

            viewModel.StorageFrom = new StorageViewModel
            {
                _id = model.StorageFromId.ToString(),
                code = model.StorageFromCode,
                name = model.StorageFromName
            };

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseReceiptFabricItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseReceiptFabricItemViewModel viewModelItem = new GarmentLeftoverWarehouseReceiptFabricItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseReceiptFabricItem, GarmentLeftoverWarehouseReceiptFabricItemViewModel>.Copy(modelItem, viewModelItem);

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

        public ReadResponse<GarmentLeftoverWarehouseReceiptFabric> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseReceiptFabric> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "ReceiptNoteNo", "UnitFromName", "UENNo", "StorageFromName"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptFabric>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptFabric>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptFabric>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "ReceiptNoteNo", "UnitFrom", "UENNo", "StorageFrom", "ReceiptDate"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseReceiptFabric
            {
                Id = s.Id,
                ReceiptNoteNo = s.ReceiptNoteNo,
                UnitFromId = s.UnitFromId,
                UnitFromCode = s.UnitFromCode,
                UnitFromName = s.UnitFromName,
                UENNo = s.UENNo,
                StorageFromId = s.StorageFromId,
                StorageFromCode = s.StorageFromCode,
                StorageFromName = s.StorageFromName,
                ReceiptDate = s.ReceiptDate
            });

            Pageable<GarmentLeftoverWarehouseReceiptFabric> pageable = new Pageable<GarmentLeftoverWarehouseReceiptFabric>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseReceiptFabric> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseReceiptFabric>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseReceiptFabric> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(GarmentLeftoverWarehouseReceiptFabric model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Created = 0;

                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.ReceiptNoteNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Create Penerimaan Gudang Sisa Fabric - " + model.ReceiptNoteNo);

                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            PONo = item.POSerialNumber,
                            UomId = item.UomId,
                            UomUnit = item.UomUnit,
                            Quantity = item.Quantity,
                            ProductCode=item.ProductCode,
                            ProductId=item.ProductId,
                            ProductName=item.ProductName,
                            BasicPrice=item.BasicPrice
                        };
                        await StockService.StockIn(stock, model.ReceiptNoteNo, model.Id, item.Id);
                    }

                    await UpdateUnitExpenditureNoteIsReceived(model.UENId, true);

                    transaction.Commit();

                    return Created;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseReceiptFabric model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Updated = 0;

                    GarmentLeftoverWarehouseReceiptFabric existingModel = await DbSet.Where(w => w.Id == id).FirstOrDefaultAsync();
                    if (existingModel.ReceiptDate != model.ReceiptDate)
                    {
                        existingModel.ReceiptDate = model.ReceiptDate;
                    }
                    if (existingModel.Remark != model.Remark)
                    {
                        existingModel.Remark = model.Remark;
                    }

                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Update Penerimaan Gudang Sisa Fabric - " + model.ReceiptNoteNo);

                    Updated = await DbContext.SaveChangesAsync();
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

        public async Task<int> DeleteAsync(int id)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Deleted = 0;

                    GarmentLeftoverWarehouseReceiptFabric model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Delete Penerimaan Gudang Sisa Fabric - " + model.ReceiptNoteNo);

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            PONo = item.POSerialNumber,
                            UomId = item.UomId,
                            UomUnit = item.UomUnit,
                            Quantity = item.Quantity,
                            ProductCode = item.ProductCode,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            BasicPrice = item.BasicPrice
                        };
                        await StockService.StockOut(stock, model.ReceiptNoteNo, model.Id, item.Id);
                    }

                    await UpdateUnitExpenditureNoteIsReceived(model.UENId, false);

                    transaction.Commit();

                    return Deleted;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        private string GenerateNo(GarmentLeftoverWarehouseReceiptFabric model)
        {
            string prefix = "BMF" + model.UnitFromCode.Trim() + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.ReceiptNoteNo.StartsWith(prefix))
                .OrderByDescending(o => o.ReceiptNoteNo)
                .Select(s => int.Parse(s.ReceiptNoteNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D4")}";

            return curNo;
        }

        private async Task UpdateUnitExpenditureNoteIsReceived(long UENId, bool IsReceived)
        {

            var stringContentRequest = JsonConvert.SerializeObject(new List<object>
            {
                new { op = "replace", path = "/IsReceived", value = IsReceived }
            });
            var httpContentRequest = new StringContent(stringContentRequest, Encoding.UTF8, General.JsonMediaType);

            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var response = await httpService.PatchAsync(GarmentUnitReceiptNoteUri + UENId, httpContentRequest);
            if (!response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentResponse) ?? new Dictionary<string, object>();

                throw new Exception(string.Concat("Error from '", GarmentUnitReceiptNoteUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
            }
        }
    }
}
