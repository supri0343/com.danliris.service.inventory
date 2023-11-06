using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories;
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

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class GarmentLeftoverWarehouseReceiptAccessoriesService : IGarmentLeftoverWarehouseReceiptAccessoriesService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptAccessoriesService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptAccessory> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;
        private readonly IGarmentLeftoverWarehouseStockService StockService;
        private readonly string GarmentUnitReceiptNoteUri;
        private readonly ILogHistoryService logHistoryService;
        public GarmentLeftoverWarehouseReceiptAccessoriesService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptAccessory>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
            GarmentUnitReceiptNoteUri = APIEndpoint.Purchasing + "garment-unit-expenditure-notes/";
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
        }
        public async Task<int> CreateAsync(GarmentLeftoverWarehouseReceiptAccessory model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Created = 0;

                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.InvoiceNoReceive = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Create Penerimaan Gudang Sisa Accessoris - " + model.InvoiceNoReceive);

                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES,
                            UnitId = model.RequestUnitId,
                            UnitCode = model.RequestUnitCode,
                            UnitName = model.RequestUnitName,
                            PONo = item.POSerialNumber,
                            UomId = item.UomUnitId,
                            UomUnit = item.UomUnit,
                            Quantity = item.Quantity,
                            ProductCode=item.ProductCode,
                            ProductId=item.ProductId,
                            ProductName=item.ProductName,
                            BasicPrice = item.BasicPrice
                        };
                        await StockService.StockIn(stock, model.InvoiceNoReceive, model.Id, item.Id);
                    }

                    await UpdateUnitExpenditureNoteIsReceived(model.UENid, true);

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

        public async Task<int> DeleteAsync(int id)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Deleted = 0;

                    GarmentLeftoverWarehouseReceiptAccessory model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Delete Penerimaan Gudang Sisa Accessoris - " + model.InvoiceNoReceive);

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES,
                            UnitId = model.RequestUnitId,
                            UnitCode = model.RequestUnitCode,
                            UnitName = model.RequestUnitName,
                            PONo = item.POSerialNumber,
                            UomId = item.UomUnitId,
                            UomUnit = item.UomUnit,
                            Quantity = item.Quantity,
                            ProductCode = item.ProductCode,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName
                        };
                        await StockService.StockOut(stock, model.InvoiceNoReceive, model.Id, item.Id);
                    }

                    await UpdateUnitExpenditureNoteIsReceived(model.UENid, false);

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

        public GarmentLeftoverWarehouseReceiptAccessory MapToModel(GarmentLeftoverWarehouseReceiptAccessoriesViewModel viewModel)
        {
            GarmentLeftoverWarehouseReceiptAccessory model = new GarmentLeftoverWarehouseReceiptAccessory();
            PropertyCopier<GarmentLeftoverWarehouseReceiptAccessoriesViewModel, GarmentLeftoverWarehouseReceiptAccessory>.Copy(viewModel, model);

            if (viewModel.RequestUnit != null)
            {
                model.RequestUnitId = long.Parse(viewModel.RequestUnit.Id);
                model.RequestUnitCode = viewModel.RequestUnit.Code;
                model.RequestUnitName = viewModel.RequestUnit.Name;
            }

            if (viewModel.Storage != null)
            {
                model.StorageFromId = long.Parse(viewModel.Storage._id);
                model.StorageFromCode = viewModel.Storage.code;
                model.StorageFromName = viewModel.Storage.name;
            }

            model.Items = new List<GarmentLeftoverWarehouseReceiptAccessoryItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseReceiptAccessoryItem modelItem = new GarmentLeftoverWarehouseReceiptAccessoryItem();
                PropertyCopier<GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel, GarmentLeftoverWarehouseReceiptAccessoryItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Product != null)
                {
                    modelItem.ProductId = long.Parse(viewModelItem.Product.Id);
                    modelItem.ProductCode = viewModelItem.Product.Code;
                    modelItem.ProductName = viewModelItem.Product.Name;
                }

                if (viewModelItem.Uom != null)
                {
                    modelItem.UomUnitId = long.Parse(viewModelItem.Uom.Id);
                    modelItem.UomUnit = viewModelItem.Uom.Unit;
                }

                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseReceiptAccessoriesViewModel MapToViewModel(GarmentLeftoverWarehouseReceiptAccessory model)
        {
            GarmentLeftoverWarehouseReceiptAccessoriesViewModel viewModel = new GarmentLeftoverWarehouseReceiptAccessoriesViewModel();
            PropertyCopier<GarmentLeftoverWarehouseReceiptAccessory, GarmentLeftoverWarehouseReceiptAccessoriesViewModel>.Copy(model, viewModel);

            viewModel.RequestUnit = new UnitViewModel
            {
                Id = model.RequestUnitId.ToString(),
                Code = model.RequestUnitCode,
                Name = model.RequestUnitName
            };

            viewModel.Storage = new StorageViewModel
            {
                _id = model.StorageFromId.ToString(),
                code = model.StorageFromCode,
                name = model.StorageFromName
            };

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel viewModelItem = new GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseReceiptAccessoryItem, GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel>.Copy(modelItem, viewModelItem);

                    viewModelItem.Product = new ProductViewModel
                    {
                        Id = modelItem.ProductId.ToString(),
                        Code = modelItem.ProductCode,
                        Name = modelItem.ProductName
                    };

                    viewModelItem.Uom = new UomViewModel
                    {
                        Id = modelItem.UomUnitId.ToString(),
                        Unit = modelItem.UomUnit
                    };

                    viewModel.Items.Add(viewModelItem);
                }
            }
            return viewModel;
        }

        public ReadResponse<GarmentLeftoverWarehouseReceiptAccessory> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseReceiptAccessory> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "InvoiceNoReceive", "RequestUnitName", "UENNo", "StorageFromName"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptAccessory>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptAccessory>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptAccessory>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "InvoiceNoReceive", "RequestUnitName", "UENNo", "StorageFromName", "StorageReceiveDate"
            };

            //Query = Query.Select(s => new GarmentLeftoverWarehouseReceiptAccessory
            //{
            //    Id = s.Id,
            //    ReceiptDate = s.ReceiptDate,
            //    UnitFromId = s.UnitFromId,
            //    UnitFromCode = s.UnitFromCode,
            //    UnitFromName = s.UnitFromName,
            //    UENNo = s.UENNo,
            //    StorageFromId = s.StorageFromId,
            //    StorageFromCode = s.StorageFromCode,
            //    StorageFromName = s.StorageFromName,
            //    ReceiptDate = s.ReceiptDate
            //});

            Pageable<GarmentLeftoverWarehouseReceiptAccessory> pageable = new Pageable<GarmentLeftoverWarehouseReceiptAccessory>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseReceiptAccessory> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseReceiptAccessory>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseReceiptAccessory> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseReceiptAccessory model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseReceiptAccessory existingModel = await DbSet.Where(w => w.Id == id).FirstOrDefaultAsync();
                    //if (existingModel.InvoiceNoReceive != model.InvoiceNoReceive)
                    //{
                    //    existingModel.InvoiceNoReceive = model.InvoiceNoReceive;
                    //}
                    //if (existingModel.RequestUnitCode != model.RequestUnitCode)
                    //{
                    //    existingModel.RequestUnitCode = model.RequestUnitCode;
                    //}
                    //if (existingModel.RequestUnitId != model.RequestUnitId)
                    //{
                    //    existingModel.RequestUnitId = model.RequestUnitId;
                    //}
                    //if (existingModel.RequestUnitName != model.RequestUnitName)
                    //{
                    //    existingModel.RequestUnitName = model.RequestUnitName;
                    //}
                    //if (existingModel.StorageFromName != model.StorageFromName)
                    //{
                    //    existingModel.StorageFromName = model.StorageFromName;
                    //}
                    //if (existingModel.StorageFromId != model.StorageFromId)
                    //{
                    //    existingModel.StorageFromId = model.StorageFromId;
                    //}
                    //if (existingModel.StorageFromCode != model.StorageFromCode)
                    //{
                    //    existingModel.StorageFromCode = model.StorageFromCode;
                    //}
                    if (existingModel.StorageReceiveDate != model.StorageReceiveDate)
                    {
                        existingModel.StorageReceiveDate = model.StorageReceiveDate;
                    }
                    //if (existingModel.UENid != model.UENid)
                    //{
                    //    existingModel.UENid = model.UENid;
                    //}
                    //if (existingModel.UENNo != model.UENNo)
                    //{
                    //    existingModel.UENNo = model.UENNo;
                    //}
                    if (existingModel.Remark != model.Remark)
                    {
                        existingModel.Remark = model.Remark;
                    }

                    //if (model.AvalType == "AVAL FABRIC")
                    //{
                    //    if (existingModel.TotalAval != model.TotalAval)
                    //    {
                    //        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                    //        {
                    //            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                    //            UnitId = model.UnitFromId,
                    //            UnitCode = model.UnitFromCode,
                    //            UnitName = model.UnitFromName,
                    //            Quantity = existingModel.TotalAval
                    //        };
                    //        await StockService.StockOut(stock, existingModel.AvalReceiptNo, model.Id, 0);

                    //        GarmentLeftoverWarehouseStock stock1 = new GarmentLeftoverWarehouseStock
                    //        {
                    //            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC,
                    //            UnitId = model.UnitFromId,
                    //            UnitCode = model.UnitFromCode,
                    //            UnitName = model.UnitFromName,
                    //            Quantity = model.TotalAval
                    //        };
                    //        await StockService.StockIn(stock1, model.AvalReceiptNo, model.Id, 0);
                    //        existingModel.TotalAval = model.TotalAval;
                    //    }
                    //}


                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Update Penerimaan Gudang Sisa Accessoris - " + model.InvoiceNoReceive);

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

        private string GenerateNo(GarmentLeftoverWarehouseReceiptAccessory model)
        {
            string code = "BMACC";
            string prefix = code + model.RequestUnitCode.Trim() + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.InvoiceNoReceive.StartsWith(prefix))
                .OrderByDescending(o => o.InvoiceNoReceive)
                .Select(s => int.Parse(s.InvoiceNoReceive.Replace(prefix, "")))
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
