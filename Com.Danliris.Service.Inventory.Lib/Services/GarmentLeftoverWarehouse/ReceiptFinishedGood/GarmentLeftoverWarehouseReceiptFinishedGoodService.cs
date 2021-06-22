using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodService : IGarmentLeftoverWarehouseReceiptFinishedGoodService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptFinishedGoodService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptFinishedGood> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        private readonly string GarmentExpenditureGoodUri;

        public GarmentLeftoverWarehouseReceiptFinishedGoodService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptFinishedGood>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
            GarmentExpenditureGoodUri = APIEndpoint.GarmentProduction + "expenditure-goods/";
        }

        public GarmentLeftoverWarehouseReceiptFinishedGood MapToModel(GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel)
        {
            GarmentLeftoverWarehouseReceiptFinishedGood model = new GarmentLeftoverWarehouseReceiptFinishedGood();
            PropertyCopier<GarmentLeftoverWarehouseReceiptFinishedGoodViewModel, GarmentLeftoverWarehouseReceiptFinishedGood>.Copy(viewModel, model);

            if (viewModel.UnitFrom != null)
            {
                model.UnitFromId = long.Parse(viewModel.UnitFrom.Id);
                model.UnitFromCode = viewModel.UnitFrom.Code;
                model.UnitFromName = viewModel.UnitFrom.Name;
            }

            model.Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseReceiptFinishedGoodItem modelItem = new GarmentLeftoverWarehouseReceiptFinishedGoodItem();
                PropertyCopier<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel, GarmentLeftoverWarehouseReceiptFinishedGoodItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Size != null)
                {
                    modelItem.SizeId = viewModelItem.Size.Id;
                    modelItem.SizeName = viewModelItem.Size.Name;
                }

                if (viewModelItem.Uom != null)
                {
                    modelItem.UomId = long.Parse(viewModelItem.Uom.Id);
                    modelItem.UomUnit = viewModelItem.Uom.Unit;
                }

                if (viewModelItem.LeftoverComodity != null)
                {
                    modelItem.LeftoverComodityCode = viewModelItem.LeftoverComodity.Code;
                    modelItem.LeftoverComodityId = viewModelItem.LeftoverComodity.Id;
                    modelItem.LeftoverComodityName = viewModelItem.LeftoverComodity.Name;
                }

                if (viewModelItem.Buyer != null)
                {
                    modelItem.BuyerId = viewModelItem.Buyer.Id;
                    modelItem.BuyerCode = viewModelItem.Buyer.Code;
                    modelItem.BuyerName = viewModelItem.Buyer.Name;
                }

                if (viewModelItem.Comodity != null)
                {
                    modelItem.ComodityId = viewModelItem.Comodity.Id;
                    modelItem.ComodityCode = viewModelItem.Comodity.Code;
                    modelItem.ComodityName = viewModelItem.Comodity.Name;
                }

                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseReceiptFinishedGoodViewModel MapToViewModel(GarmentLeftoverWarehouseReceiptFinishedGood model)
        {
            GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel();
            PropertyCopier<GarmentLeftoverWarehouseReceiptFinishedGood, GarmentLeftoverWarehouseReceiptFinishedGoodViewModel>.Copy(model, viewModel);

            viewModel.UnitFrom = new UnitViewModel
            {
                Id = model.UnitFromId.ToString(),
                Code = model.UnitFromCode,
                Name = model.UnitFromName
            };


            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel viewModelItem = new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseReceiptFinishedGoodItem, GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>.Copy(modelItem, viewModelItem);

                    viewModelItem.Size = new SizeViewModel
                    {
                        Id = modelItem.SizeId,
                        Name = modelItem.SizeName
                    };

                    viewModelItem.Uom = new UomViewModel
                    {
                        Id = modelItem.UomId.ToString(),
                        Unit = modelItem.UomUnit
                    };

                    viewModelItem.LeftoverComodity = new LeftoverComodityViewModel
                    {
                        Id = modelItem.LeftoverComodityId,
                        Name = modelItem.LeftoverComodityName,
                        Code = modelItem.LeftoverComodityCode
                    };

                    viewModelItem.Buyer = new BuyerViewModel
                    {
                        Id = modelItem.BuyerId,
                        Code = modelItem.BuyerCode,
                        Name = modelItem.BuyerName
                    };

                    viewModelItem.Comodity = new ComodityViewModel
                    {
                        Id = modelItem.ComodityId,
                        Code = modelItem.ComodityCode,
                        Name = modelItem.ComodityName
                    };

                    viewModel.Items.Add(viewModelItem);
                }
            }

            return viewModel;
        }

        public ReadResponse<GarmentLeftoverWarehouseReceiptFinishedGood> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseReceiptFinishedGood> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "FinishedGoodReceiptNo", "UnitFromName"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptFinishedGood>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptFinishedGood>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseReceiptFinishedGood>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "FinishedGoodReceiptNo", "UnitFrom", "ReceiptDate"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseReceiptFinishedGood
            {
                Id = s.Id,
                FinishedGoodReceiptNo = s.FinishedGoodReceiptNo,
                UnitFromId = s.UnitFromId,
                UnitFromCode = s.UnitFromCode,
                UnitFromName = s.UnitFromName,
                ReceiptDate = s.ReceiptDate
            });

            Pageable<GarmentLeftoverWarehouseReceiptFinishedGood> pageable = new Pageable<GarmentLeftoverWarehouseReceiptFinishedGood>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseReceiptFinishedGood> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseReceiptFinishedGood>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseReceiptFinishedGood> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(GarmentLeftoverWarehouseReceiptFinishedGood model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.FinishedGoodReceiptNo = GenerateNo(model);

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
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            RONo = item.RONo,
                            LeftoverComodityCode = item.LeftoverComodityCode,
                            LeftoverComodityId = item.LeftoverComodityId,
                            LeftoverComodityName = item.LeftoverComodityName,
                            Quantity = item.Quantity,
                            BasicPrice = item.BasicPrice
                        };
                        await StockService.StockIn(stock, model.FinishedGoodReceiptNo, model.Id, item.Id);
                        await UpdateExpenditureGoodIsReceived(item.ExpenditureGoodId, "true");
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

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseReceiptFinishedGood model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseReceiptFinishedGood existingModel = await ReadByIdAsync(id);
                    if (existingModel.ReceiptDate != model.ReceiptDate)
                    {
                        existingModel.ReceiptDate = model.ReceiptDate;
                    }
                    if (existingModel.Description != model.Description)
                    {
                        existingModel.Description = model.Description;
                    }
                    foreach (var existingItem in existingModel.Items)
                    {
                        GarmentLeftoverWarehouseStock stockOut = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = existingModel.UnitFromId,
                            UnitCode = existingModel.UnitFromCode,
                            UnitName = existingModel.UnitFromName,
                            RONo = existingItem.RONo,
                            Quantity = existingItem.Quantity,
                            LeftoverComodityCode = existingItem.LeftoverComodityCode,
                            LeftoverComodityId = existingItem.LeftoverComodityId,
                            LeftoverComodityName = existingItem.LeftoverComodityName,
                            BasicPrice = existingItem.BasicPrice
                        };

                        await StockService.StockOut(stockOut, model.FinishedGoodReceiptNo, model.Id, existingItem.Id);
                    }

                    foreach (var existingItem in existingModel.Items)
                    {
                        var item = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                        if (item == null)
                        {
                            existingItem.FlagForDelete(IdentityService.Username, UserAgent);
                            await UpdateExpenditureGoodIsReceived(existingItem.ExpenditureGoodId, "false");
                        }
                        else
                        {
                            if (existingItem.Quantity != item.Quantity)
                            {
                                existingItem.Quantity = item.Quantity;
                            }
                            existingItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                    }

                    foreach (var item in model.Items.Where(i => i.Id == 0))
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        await UpdateExpenditureGoodIsReceived(item.ExpenditureGoodId, "true");
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        existingModel.Items.Add(item);
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = existingModel.UnitFromId,
                            UnitCode = existingModel.UnitFromCode,
                            UnitName = existingModel.UnitFromName,
                            RONo = item.RONo,
                            Quantity = item.Quantity,
                            LeftoverComodityCode = item.LeftoverComodityCode,
                            LeftoverComodityId = item.LeftoverComodityId,
                            LeftoverComodityName = item.LeftoverComodityName,
                            BasicPrice=item.BasicPrice
                        };

                        await StockService.StockIn(stock, model.FinishedGoodReceiptNo, model.Id, item.Id);
                    }
                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

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
                    GarmentLeftoverWarehouseReceiptFinishedGood model = await ReadByIdAsync(id);
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
                            UnitId = model.UnitFromId,
                            UnitCode = model.UnitFromCode,
                            UnitName = model.UnitFromName,
                            RONo = item.RONo,
                            LeftoverComodityCode = item.LeftoverComodityCode,
                            LeftoverComodityId = item.LeftoverComodityId,
                            LeftoverComodityName = item.LeftoverComodityName,
                            Quantity = item.Quantity,
                            BasicPrice = item.BasicPrice
                        };

                        await StockService.StockOut(stock, model.FinishedGoodReceiptNo, model.Id, item.Id);
                        await UpdateExpenditureGoodIsReceived(item.ExpenditureGoodId, "false");
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

        private string GenerateNo(GarmentLeftoverWarehouseReceiptFinishedGood model)
        {
            string prefix = "BMB" + model.UnitFromCode.Trim() + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.FinishedGoodReceiptNo.StartsWith(prefix))
                .OrderByDescending(o => o.FinishedGoodReceiptNo)
                .Select(s => int.Parse(s.FinishedGoodReceiptNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D4")}";

            return curNo;
        }

        private async Task UpdateExpenditureGoodIsReceived(Guid ExGoodId, string IsReceived)
        {

            var stringContentRequest = IsReceived;
            var httpContentRequest = new StringContent(stringContentRequest, Encoding.UTF8, General.JsonMediaType);

            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var response = await httpService.PutAsync(GarmentExpenditureGoodUri + "update-received/" + ExGoodId, httpContentRequest);
            if (!response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentResponse) ?? new Dictionary<string, object>();

                throw new Exception(string.Concat("Error from '", GarmentExpenditureGoodUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
            }
        }

        public IQueryable<ReceiptFinishedGoodMonitoringViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
            int index = 0;
            var qA = from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                     where data.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
                     && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                     select new { data.FinishedGoodReceiptNo, data.ReceiptDate, data.UnitFromCode, data.Id };
            var Query = from a in qA
                        join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                        select new ReceiptFinishedGoodMonitoringViewModel
                        {
                            ReceiptNoteNo = a.FinishedGoodReceiptNo,
                            ReceiptDate = a.ReceiptDate,
                            UnitFromCode = a.UnitFromCode,
                            ExpenditureGoodNo = b.ExpenditureGoodNo,
                            ComodityName = b.ComodityName,
                            Quantity = b.Quantity,
                            RONo = b.RONo,
                            UomUnit = b.UomUnit
                        };
            var querySum= Query
                .GroupBy(x => new { x.ReceiptNoteNo, x.ReceiptDate, x.UnitFromCode, x.ExpenditureGoodNo, x.ComodityName, x.RONo,x.UomUnit }, (key, group) => new
            ReceiptFinishedGoodMonitoringViewModel
            {
                ReceiptNoteNo = key.ReceiptNoteNo,
                ReceiptDate = key.ReceiptDate,
                UnitFromCode = key.UnitFromCode,
                ExpenditureGoodNo = key.ExpenditureGoodNo,
                ComodityName = key.ComodityName,
                Quantity = group.Sum(s=>s.Quantity),
                RONo = key.RONo,
                UomUnit = key.UomUnit
            }).OrderBy(s => s.ReceiptDate);
            return querySum;
        }

        public Tuple<List<ReceiptFinishedGoodMonitoringViewModel>, int> GetMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.ReceiptDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<ReceiptFinishedGoodMonitoringViewModel> pageable = new Pageable<ReceiptFinishedGoodMonitoringViewModel>(Query, page - 1, size);
            List<ReceiptFinishedGoodMonitoringViewModel> Data = pageable.Data.ToList<ReceiptFinishedGoodMonitoringViewModel>();

            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });

            if (page == ((TotalData / size) + 1) && TotalData != 0)
            {
                var QtyTotal = Query.Sum(x => x.Quantity);
                //var WeightTotal = Query.Sum(x => x.Weight);
                ReceiptFinishedGoodMonitoringViewModel vm = new ReceiptFinishedGoodMonitoringViewModel();

                vm.ReceiptNoteNo = "T O T A L .........";
                vm.ReceiptDate = DateTimeOffset.MinValue;
                vm.UnitFromCode = "";
                vm.ExpenditureGoodNo = "";
                vm.ComodityName = "";
                vm.Quantity = QtyTotal;
                vm.RONo = "";
                vm.UomUnit = "";

                Data.Add(vm);

            }
            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.ReceiptDate);

            var QtyTotal = Query.Sum(x => x.Quantity);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Pengeluaran Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.ReceiptNoteNo, item.ReceiptDate.AddHours(offset).ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.UnitFromCode, item.ExpenditureGoodNo, item.RONo, item.ComodityName, item.Quantity, item.UomUnit);
                }

                result.Rows.Add("" , "T O T A L .......", "", "", "", "", "", QtyTotal, "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Pengeluaran Gudang Sisa Barang Jadi") }, true);

        }
    }
}