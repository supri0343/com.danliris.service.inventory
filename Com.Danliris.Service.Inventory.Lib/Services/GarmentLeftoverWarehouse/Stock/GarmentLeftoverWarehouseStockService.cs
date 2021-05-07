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
                "Id", "ReferenceType", "Unit", "PONo","RONo", "Product", "Uom", "Quantity"
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
                        Query = Query.Where(w => w.PONo == stock.PONo && w.UomId == stock.UomId);
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
                        Query = Query.Where(w => w.PONo == stock.PONo && w.UomId == stock.UomId);
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

        
        public IQueryable<GarmentLeftoverWarehouseStockMonitoringViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo,int UnitId, string type,int offset)
        {

            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
           
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> garmentLeftoverWarehouseStockMonitoringViewModel = new List<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            if (type == "FABRIC")
            {
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date < DateTo.Date
                                         && data.UnitFromId == UnitId
                                              select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       PONo = b.POSerialNumber,
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = a.UnitFromCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date < DateTo.Date
                                           
                                                  select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems where expend.UnitId == UnitId
                                                  select new { expend.UomUnit,expend.PONo,expend.Quantity,expend.UnitCode,expend.ExpenditureId}) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                       {
                                           PONo = b.PONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0,
                                           QuantityReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                           UomUnit = b.UomUnit,
                                           UnitCode = b.UnitCode,
                                           ProductCode = "",
                                           ProductRemark = "",
                                           ProductName = "",
                                           FabricRemark = "",
                                           EndbalanceQty = 0,
                                           index = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.PONo, x.UnitCode, x.UomUnit, x.index }, (key, group) => new
                    {
                        pono = key.PONo,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        expend = group.Sum(s => s.QuantityExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        uomunit = key.UomUnit,
                        unit = key.UnitCode,
                        index = key.index
                    }).OrderBy(s => s.pono);


                foreach (var data in querySum)
                {
                    GarmentLeftoverWarehouseStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseStockMonitoringViewModel
                    {
                        PONo = data.pono,
                        BeginingbalanceQty = data.begining,
                        QuantityReceipt = data.receipt,
                        QuantityExpend = data.expend,
                        UnitCode = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems where aa.POSerialNumber == data.pono select aa.ProductCode).FirstOrDefault(),
                        ProductName = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems where aa.POSerialNumber == data.pono select aa.ProductName).FirstOrDefault(),
                        ProductRemark = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems where aa.POSerialNumber == data.pono select aa.ProductRemark).FirstOrDefault(),
                        FabricRemark = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems where aa.POSerialNumber == data.pono select aa.FabricRemark).FirstOrDefault(),
                        EndbalanceQty = data.begining + data.receipt - data.expend
                    };
                    garmentLeftoverWarehouseStockMonitoringViewModel.Add(garmentLeftover);
                }
            }
            else
            {
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                              where data._IsDeleted == false
                                         && data.StorageReceiveDate.AddHours(offset).Date < DateTo.Date
                                         && data.RequestUnitId == UnitId
                                              select new { data.RequestUnitCode, data.StorageReceiveDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       PONo = b.POSerialNumber,
                                       BeginingbalanceQty = a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       QuantityReceipt = a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = a.RequestUnitCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date < DateTo.Date
                                            
                                                  select new {  data.ExpenditureDate, data.Id })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
                                                  where expend.UnitId== UnitId
                                                  select new { expend.ExpenditureId, expend.UomUnit,expend.UnitCode,expend.Quantity,expend.PONo}
                                                  )on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                       {
                                           PONo = b.PONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0,
                                           QuantityReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                           UomUnit = b.UomUnit,
                                           UnitCode =b.UnitCode ,
                                           ProductCode = "",
                                           ProductRemark = "",
                                           ProductName = "",
                                           FabricRemark = "",
                                           EndbalanceQty = 0,
                                           index = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.PONo, x.UnitCode, x.UomUnit, x.index }, (key, group) => new
                    {
                        pono = key.PONo,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        expend = group.Sum(s => s.QuantityExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        uomunit = key.UomUnit,
                        unit = key.UnitCode,
                        index = key.index
                    }).OrderBy(s => s.pono);


                foreach (var data in querySum)
                {
                    GarmentLeftoverWarehouseStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseStockMonitoringViewModel
                    {
                        PONo = data.pono,
                        BeginingbalanceQty = data.begining,
                        QuantityReceipt = data.receipt,
                        QuantityExpend = data.expend,
                        UnitCode = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems where aa.POSerialNumber == data.pono select aa.ProductCode).FirstOrDefault(),
                        ProductName = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems where aa.POSerialNumber == data.pono select aa.ProductName).FirstOrDefault(),
                        ProductRemark = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems where aa.POSerialNumber == data.pono select aa.ProductRemark).FirstOrDefault(),
                        FabricRemark = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems where aa.POSerialNumber == data.pono select aa.Remark).FirstOrDefault(),
                        EndbalanceQty = data.begining + data.receipt - data.expend
                    };
                    garmentLeftoverWarehouseStockMonitoringViewModel.Add(garmentLeftover);
                }
            }
            return garmentLeftoverWarehouseStockMonitoringViewModel.AsQueryable();
        }

        public Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringFabric(DateTime? dateFrom, DateTime? dateTo,int unitId, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId,"FABRIC",offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.PONo);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });
            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelFabric(DateTime? dateFrom, DateTime? dateTo,int unitId, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo,unitId,"FABRIC", offset);
            Query = Query.OrderByDescending(b => b.PONo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0,0,0,0,0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.UnitCode, item.PONo, item.ProductCode, item.ProductName, item.ProductRemark, item.FabricRemark, item.BeginingbalanceQty, item.QuantityReceipt,item.QuantityExpend,item.EndbalanceQty,item.UomUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Stock Gudang Sisa - FABRIC") }, true);

        }

        public MemoryStream GenerateExcelAcc(DateTime? dateFrom, DateTime? dateTo,int unitId, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "ACC", offset);
            Query = Query.OrderByDescending(b => b.PONo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.UnitCode, item.PONo, item.ProductCode, item.ProductName, item.ProductRemark, item.BeginingbalanceQty, item.QuantityReceipt, item.QuantityExpend, item.EndbalanceQty, item.UomUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Stock Gudang Sisa - ACC") }, true);

        }

        public Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringAcc(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "ACC", offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.PONo);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });
            return Tuple.Create(Data, TotalData);
        }
    }
}
