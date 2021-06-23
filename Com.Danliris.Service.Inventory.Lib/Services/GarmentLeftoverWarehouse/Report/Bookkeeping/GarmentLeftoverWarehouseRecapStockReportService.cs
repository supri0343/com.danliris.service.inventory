using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseRecapStockReportService : IGarmentLeftoverWarehouseRecapStockReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseRecapStockReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseRecapStockReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public IQueryable<GarmentLeftoverWarehouseRecapStockReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset, string typeAval = "")
        {

            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;


            List<GarmentLeftoverWarehouseRecapStockReportViewModel> GarmentLeftoverWarehouseRecapStockReportViewModel = new List<GarmentLeftoverWarehouseRecapStockReportViewModel>();

            var QueryBalanceFABRIC = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                            where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
                                            select new { data._CreatedUtc, data.Id })
                                 join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                 select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                 {
                                     FabricPrice= b.Quantity * b.BasicPrice,
                                     FabricQty=b.Quantity,
                                     FabricUom=b.UomUnit,
                                     Unit=b.UnitName
                                 };
            var QueryReceiptFABRIC = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                                where data._IsDeleted == false
                                           && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                                select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                     join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                     select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                     {
                                         FabricPrice = b.Quantity * b.BasicPrice,
                                         FabricQty = b.Quantity,
                                         FabricUom = b.UomUnit,
                                         Unit = a.UnitFromName
                                     };
            var QueryExpenditureFABRIC = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                    where data._IsDeleted == false
                                                        && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                    select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                         join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                    select new { expend.BasicPrice, expend.UomUnit, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                         select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                         {
                                             FabricPrice = b.Quantity * b.BasicPrice,
                                             FabricQty = b.Quantity,
                                             FabricUom = b.UomUnit,
                                             Unit = b.UnitName
                                         };
            
                var QueryFABRIC = QueryReceiptFABRIC.Union(QueryExpenditureFABRIC).Union(QueryBalanceFABRIC);
                //var querySum = QueryFABRIC.ToList()
                //    .GroupBy(x => new { x.PONo, x.UnitName, x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
                //    {
                //        pono = key.PONo,
                //        begining = group.Sum(s => s.BeginingbalanceQty),
                //        beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                //        expendunit = group.Sum(s => s.QuantityUnitExpend),
                //        expendunitPrice = group.Sum(s => s.PriceUnitExpend),
                //        expendsample = group.Sum(s => s.QuantitySampleExpend),
                //        expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
                //        expendother = group.Sum(s => s.QuantityOtherExpend),
                //        expendotherPrice = group.Sum(s => s.PriceOtherExpend),
                //        expendlocal = group.Sum(s => s.QuantityLocalExpend),
                //        expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
                //        receipt = group.Sum(s => s.QuantityReceipt),
                //        priceReceipt = group.Sum(s => s.PriceReceipt),
                //        basicprice = group.Sum(s => s.BasicPrice),
                //        uomunit = key.UomUnit,
                //        unit = key.UnitName,
                //        productCode = key.ProductCode,
                //        productName = key.ProductName
                //    }).OrderBy(s => s.pono);

                //foreach (var data in querySum)
                //{

                //    GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                //    {
                //        PONo = data.pono,
                //        BeginingbalanceQty = data.begining,
                //        BeginingbalancePrice = data.beginingPrice,
                //        QuantityReceipt = data.receipt,
                //        PriceReceipt = data.priceReceipt,
                //        BasicPrice = data.basicprice,
                //        QuantityUnitExpend = data.expendunit,
                //        PriceUnitExpend = data.expendunitPrice,
                //        QuantityLocalExpend = data.expendlocal,
                //        PriceLocalExpend = data.expendlocalPrice,
                //        QuantityOtherExpend = data.expendother,
                //        PriceOtherExpend = data.expendotherPrice,
                //        QuantitySampleExpend = data.expendsample,
                //        PriceSampleExpend = data.expendsamplePrice,
                //        UnitName = data.unit,
                //        UomUnit = data.uomunit,
                //        ProductCode = data.productCode,
                //        ProductName = data.productName,

                //        EndbalanceQty = data.begining + data.receipt - data.expendlocal - data.expendother - data.expendsample - data.expendunit,
                //        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendlocalPrice - data.expendotherPrice - data.expendsamplePrice - data.expendunitPrice
                //    };
                //    GarmentLeftoverWarehouseRecapStockReportViewModel.Add(garmentLeftover);
                //}
            
            return GarmentLeftoverWarehouseRecapStockReportViewModel.AsQueryable();
        }

        public Tuple<List<GarmentLeftoverWarehouseRecapStockReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            throw new NotImplementedException();
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
