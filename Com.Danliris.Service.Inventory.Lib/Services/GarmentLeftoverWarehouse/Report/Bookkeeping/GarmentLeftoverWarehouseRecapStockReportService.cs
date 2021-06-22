using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseRecapStockReportService
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

        //public IQueryable<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> GetReportQuery(string category, DateTime? dateFrom, DateTime? dateTo, int UnitId, int offset, string typeAval = "")
        //{

        //    DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
        //    DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;


        //    List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> GarmentLeftoverWarehouseFlowStockMonitoringViewModel = new List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>();

        //    if (category == "FABRIC")
        //    {
        //        var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
        //                                      where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
        //                                      select new { data._CreatedUtc, data.Id })
        //                           join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
        //                           where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
        //                           select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                           {
        //                               PONo = b.PONo,
        //                               BeginingbalanceQty = b.Quantity,
        //                               BeginingbalancePrice = b.Quantity * b.BasicPrice,
        //                               QuantityReceipt = 0,
        //                               PriceReceipt = 0,
        //                               QuantityUnitExpend = 0,
        //                               PriceUnitExpend = 0,
        //                               QuantityLocalExpend = 0,
        //                               PriceLocalExpend = 0,
        //                               QuantityOtherExpend = 0,
        //                               PriceOtherExpend = 0,
        //                               QuantitySampleExpend = 0,
        //                               PriceSampleExpend = 0,
        //                               UomUnit = b.UomUnit,
        //                               UnitName = b.UnitName,
        //                               ProductCode = b.ProductCode,
        //                               ProductName = b.ProductName,
        //                               EndbalanceQty = 0,
        //                               BasicPrice = b.BasicPrice,
        //                           };
        //        var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
        //                                      where data._IsDeleted == false
        //                                 && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
        //                                 && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
        //                                      select new { data.UnitFromName, data.ReceiptDate, data.Id })
        //                           join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
        //                           select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                           {
        //                               PONo = b.POSerialNumber,
        //                               BeginingbalanceQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
        //                               BeginingbalancePrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
        //                               QuantityReceipt = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
        //                               PriceReceipt = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
        //                               BasicPrice = b.BasicPrice,
        //                               QuantityUnitExpend = 0,
        //                               PriceUnitExpend = 0,
        //                               QuantityLocalExpend = 0,
        //                               PriceLocalExpend = 0,
        //                               QuantityOtherExpend = 0,
        //                               PriceOtherExpend = 0,
        //                               QuantitySampleExpend = 0,
        //                               PriceSampleExpend = 0,
        //                               UomUnit = b.UomUnit,
        //                               UnitName = a.UnitFromName,
        //                               ProductCode = b.ProductCode,
        //                               ProductName = b.ProductName,
        //                               EndbalanceQty = 0

        //                           };
        //        var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
        //                                          where data._IsDeleted == false
        //                                     && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

        //                                          select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
        //                               join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
        //                                          where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
        //                                          select new { expend.BasicPrice, expend.UomUnit, expend.PONo, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
        //                               select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                               {
        //                                   PONo = b.PONo,
        //                                   BeginingbalanceQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0,
        //                                   BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0) * b.BasicPrice,
        //                                   QuantityReceipt = 0,
        //                                   PriceReceipt = 0,
        //                                   QuantityUnitExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0,
        //                                   PriceUnitExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0) * b.BasicPrice,
        //                                   QuantityLocalExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0,
        //                                   PriceLocalExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0) * b.BasicPrice,
        //                                   QuantityOtherExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0,
        //                                   PriceOtherExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0) * b.BasicPrice,
        //                                   QuantitySampleExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0,
        //                                   PriceSampleExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0) * b.BasicPrice,
        //                                   BasicPrice = b.BasicPrice,
        //                                   UomUnit = b.UomUnit,
        //                                   UnitName = b.UnitName,
        //                                   ProductCode = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems where aa._IsDeleted == false && aa.POSerialNumber == b.PONo select aa.ProductCode).FirstOrDefault() == null ? (from aa in DbContext.GarmentLeftoverWarehouseBalanceStocksItems where aa.PONo == b.PONo select aa.ProductCode).FirstOrDefault() : "",
        //                                   ProductName = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems where aa._IsDeleted == false && aa.POSerialNumber == b.PONo select aa.ProductName).FirstOrDefault() == null ? (from aa in DbContext.GarmentLeftoverWarehouseBalanceStocksItems where aa.PONo == b.PONo select aa.ProductName).FirstOrDefault() : "",
        //                                   EndbalanceQty = 0
        //                               };
        //        var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
        //        var querySum = Query.ToList()
        //            .GroupBy(x => new { x.PONo, x.UnitName, x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
        //            {
        //                pono = key.PONo,
        //                begining = group.Sum(s => s.BeginingbalanceQty),
        //                beginingPrice = group.Sum(s => s.BeginingbalancePrice),
        //                expendunit = group.Sum(s => s.QuantityUnitExpend),
        //                expendunitPrice = group.Sum(s => s.PriceUnitExpend),
        //                expendsample = group.Sum(s => s.QuantitySampleExpend),
        //                expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
        //                expendother = group.Sum(s => s.QuantityOtherExpend),
        //                expendotherPrice = group.Sum(s => s.PriceOtherExpend),
        //                expendlocal = group.Sum(s => s.QuantityLocalExpend),
        //                expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
        //                receipt = group.Sum(s => s.QuantityReceipt),
        //                priceReceipt = group.Sum(s => s.PriceReceipt),
        //                basicprice = group.Sum(s => s.BasicPrice),
        //                uomunit = key.UomUnit,
        //                unit = key.UnitName,
        //                productCode = key.ProductCode,
        //                productName = key.ProductName
        //            }).OrderBy(s => s.pono);

        //        foreach (var data in querySum)
        //        {

        //            GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //            {
        //                PONo = data.pono,
        //                BeginingbalanceQty = data.begining,
        //                BeginingbalancePrice = data.beginingPrice,
        //                QuantityReceipt = data.receipt,
        //                PriceReceipt = data.priceReceipt,
        //                BasicPrice = data.basicprice,
        //                QuantityUnitExpend = data.expendunit,
        //                PriceUnitExpend = data.expendunitPrice,
        //                QuantityLocalExpend = data.expendlocal,
        //                PriceLocalExpend = data.expendlocalPrice,
        //                QuantityOtherExpend = data.expendother,
        //                PriceOtherExpend = data.expendotherPrice,
        //                QuantitySampleExpend = data.expendsample,
        //                PriceSampleExpend = data.expendsamplePrice,
        //                UnitName = data.unit,
        //                UomUnit = data.uomunit,
        //                ProductCode = data.productCode,
        //                ProductName = data.productName,

        //                EndbalanceQty = data.begining + data.receipt - data.expendlocal - data.expendother - data.expendsample - data.expendunit,
        //                EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendlocalPrice - data.expendotherPrice - data.expendsamplePrice - data.expendunitPrice
        //            };
        //            GarmentLeftoverWarehouseFlowStockMonitoringViewModel.Add(garmentLeftover);
        //        }
        //    }
        //    else if (category == "BARANG JADI")
        //    {

        //        var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
        //                                      where data._IsDeleted == false && data.TypeOfGoods.ToString() == "BARANG JADI"
        //                                      select new { data._CreatedUtc, data.Id })
        //                           join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
        //                           where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
        //                           select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                           {
        //                               RO = b.RONo,
        //                               BeginingbalancePrice = b.Quantity * b.BasicPrice,
        //                               QuantityReceipt = 0,
        //                               PriceReceipt = 0,
        //                               QuantityUnitExpend = 0,
        //                               PriceUnitExpend = 0,
        //                               QuantityLocalExpend = 0,
        //                               PriceLocalExpend = 0,
        //                               QuantityOtherExpend = 0,
        //                               PriceOtherExpend = 0,
        //                               QuantitySampleExpend = 0,
        //                               PriceSampleExpend = 0,
        //                               UomUnit = b.UomUnit,
        //                               UnitName = b.UnitName,
        //                               ProductCode = b.LeftoverComodityCode,
        //                               ProductName = b.LeftoverComodityName,
        //                               EndbalanceQty = 0
        //                           };
        //        var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
        //                                      where data._IsDeleted == false
        //                                 && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
        //                                 && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
        //                                      select new { data.UnitFromName, data.ReceiptDate, data.Id })
        //                           join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
        //                           select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                           {
        //                               RO = b.RONo,
        //                               BeginingbalanceQty = a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
        //                               BeginingbalancePrice = (a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
        //                               QuantityReceipt = a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
        //                               PriceReceipt = (a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
        //                               QuantityUnitExpend = 0,
        //                               PriceUnitExpend = 0,
        //                               QuantityLocalExpend = 0,
        //                               PriceLocalExpend = 0,
        //                               QuantityOtherExpend = 0,
        //                               PriceOtherExpend = 0,
        //                               QuantitySampleExpend = 0,
        //                               PriceSampleExpend = 0,
        //                               BasicPrice = 0,
        //                               UomUnit = "PCS",
        //                               UnitName = a.UnitFromName,
        //                               ProductCode = b.LeftoverComodityCode,
        //                               ProductName = b.LeftoverComodityName,
        //                               EndbalanceQty = 0
        //                           };
        //        var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
        //                                          where data._IsDeleted == false
        //                                                && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
        //                                          select new { data.ExpenditureDate, data.Id, data.ExpenditureTo })
        //                               join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
        //                                          where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
        //                                          select new { expend.BasicPrice, expend.FinishedGoodExpenditureId, expend.UnitName, expend.ExpenditureQuantity, expend.RONo, expend.LeftoverComodityName }
        //                                          ) on a.Id equals b.FinishedGoodExpenditureId
        //                               select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                               {
        //                                   RO = b.RONo,
        //                                   BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0,
        //                                   BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
        //                                   QuantityReceipt = 0,
        //                                   PriceReceipt = 0,
        //                                   BasicPrice = 0,
        //                                   QuantityUnitExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "UNIT" ? b.ExpenditureQuantity : 0,
        //                                   PriceUnitExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "UNIT" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
        //                                   QuantityLocalExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "JUAL LOKAL" ? b.ExpenditureQuantity : 0,
        //                                   PriceLocalExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "JUAL LOKAL" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
        //                                   QuantityOtherExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "LAIN-LAIN" ? b.ExpenditureQuantity : 0,
        //                                   PriceOtherExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "LAIN-LAIN" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
        //                                   QuantitySampleExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "SAMPLE" ? b.ExpenditureQuantity : 0,
        //                                   PriceSampleExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "SAMPLE" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
        //                                   UomUnit = "PCS",
        //                                   UnitName = b.UnitName,
        //                                   ProductCode = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems where aa.RONo == b.RONo select aa.LeftoverComodityCode).FirstOrDefault(),
        //                                   ProductName = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems where aa.RONo == b.RONo select aa.LeftoverComodityName).FirstOrDefault(),
        //                                   EndbalanceQty = 0
        //                               };
        //        var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
        //        var querySum = Query.ToList()
        //            .GroupBy(x => new { x.RO, x.UnitName, x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
        //            {
        //                rono = key.RO,
        //                begining = group.Sum(s => s.BeginingbalanceQty),
        //                beginingPrice = group.Sum(s => s.BeginingbalancePrice),
        //                expendunit = group.Sum(s => s.QuantityUnitExpend),
        //                expendunitPrice = group.Sum(s => s.PriceUnitExpend),
        //                expendsample = group.Sum(s => s.QuantitySampleExpend),
        //                expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
        //                expendother = group.Sum(s => s.QuantityOtherExpend),
        //                expendotherPrice = group.Sum(s => s.PriceOtherExpend),
        //                expendlocal = group.Sum(s => s.QuantityLocalExpend),
        //                expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
        //                receipt = group.Sum(s => s.QuantityReceipt),
        //                priceReceipt = group.Sum(s => s.PriceReceipt),
        //                basicprice = group.Sum(s => s.BasicPrice),
        //                uomunit = key.UomUnit,
        //                unit = key.UnitName,
        //                productCode = key.ProductCode,
        //                productName = key.ProductName
        //            }).OrderBy(s => s.rono);


        //        foreach (var data in querySum)
        //        {
        //            GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //            {
        //                RO = data.rono,
        //                BeginingbalanceQty = data.begining,
        //                BeginingbalancePrice = data.beginingPrice,
        //                QuantityReceipt = data.receipt,
        //                PriceReceipt = data.priceReceipt,
        //                BasicPrice = data.basicprice,
        //                QuantityUnitExpend = data.expendunit,
        //                PriceUnitExpend = data.expendunitPrice,
        //                QuantityLocalExpend = data.expendlocal,
        //                PriceLocalExpend = data.expendlocalPrice,
        //                QuantityOtherExpend = data.expendother,
        //                PriceOtherExpend = data.expendotherPrice,
        //                QuantitySampleExpend = data.expendsample,
        //                PriceSampleExpend = data.expendsamplePrice,
        //                UnitName = data.unit,
        //                UomUnit = data.uomunit,
        //                ProductCode = data.productCode,
        //                ProductName = data.productName,
        //                EndbalanceQty = data.begining + data.receipt - data.expendlocal - data.expendother - data.expendsample - data.expendunit,
        //                EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendlocalPrice - data.expendotherPrice - data.expendsamplePrice - data.expendunitPrice

        //            };
        //            GarmentLeftoverWarehouseFlowStockMonitoringViewModel.Add(garmentLeftover);
        //        }
        //    }
        //    else
        //    {
        //        var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
        //                                      where data._IsDeleted == false && data.TypeOfGoods.ToString() == "ACCESSORIES"
        //                                      select new { data._CreatedUtc, data.Id })
        //                           join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
        //                           where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
        //                           select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                           {
        //                               PONo = b.PONo,
        //                               BeginingbalanceQty = b.Quantity,
        //                               BeginingbalancePrice = b.Quantity * b.BasicPrice,
        //                               QuantityReceipt = 0,
        //                               PriceReceipt = 0,
        //                               QuantityUnitExpend = 0,
        //                               PriceUnitExpend = 0,
        //                               QuantityLocalExpend = 0,
        //                               PriceLocalExpend = 0,
        //                               QuantityOtherExpend = 0,
        //                               PriceOtherExpend = 0,
        //                               QuantitySampleExpend = 0,
        //                               PriceSampleExpend = 0,
        //                               UomUnit = b.UomUnit,
        //                               UnitName = b.UnitName,
        //                               ProductCode = b.ProductCode,
        //                               ProductName = b.ProductName,
        //                               EndbalanceQty = 0
        //                           };
        //        var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
        //                                      where data._IsDeleted == false
        //                                 && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
        //                                 && data.RequestUnitId == (UnitId == 0 ? data.RequestUnitId : UnitId)
        //                                      select new { data.RequestUnitName, data.StorageReceiveDate, data.Id })
        //                           join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
        //                           select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                           {
        //                               PONo = b.POSerialNumber,
        //                               BeginingbalanceQty = a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
        //                               BeginingbalancePrice = (a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
        //                               QuantityReceipt = a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
        //                               PriceReceipt = (a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
        //                               QuantityUnitExpend = 0,
        //                               PriceUnitExpend = 0,
        //                               QuantityLocalExpend = 0,
        //                               PriceLocalExpend = 0,
        //                               QuantityOtherExpend = 0,
        //                               PriceOtherExpend = 0,
        //                               QuantitySampleExpend = 0,
        //                               PriceSampleExpend = 0,
        //                               UomUnit = b.UomUnit,
        //                               UnitName = a.RequestUnitName,
        //                               ProductCode = b.ProductCode,
        //                               ProductName = b.ProductName,
        //                               EndbalanceQty = 0
        //                           };
        //        var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
        //                                          where data._IsDeleted == false
        //                                     && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

        //                                          select new { data.ExpenditureDate, data.Id, data.ExpenditureDestination })
        //                               join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
        //                                          where expend.UnitId == UnitId
        //                                          select new { expend.BasicPrice, expend.ExpenditureId, expend.UomUnit, expend.UnitName, expend.Quantity, expend.PONo }
        //                                          ) on a.Id equals b.ExpenditureId
        //                               select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //                               {
        //                                   PONo = b.PONo,
        //                                   BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0,
        //                                   BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0) * b.BasicPrice,
        //                                   QuantityReceipt = 0,
        //                                   PriceReceipt = 0,
        //                                   QuantityUnitExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0,
        //                                   PriceUnitExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0) * b.BasicPrice,
        //                                   QuantityLocalExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0,
        //                                   PriceLocalExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0) * b.BasicPrice,
        //                                   QuantityOtherExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0,
        //                                   PriceOtherExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0) * b.BasicPrice,
        //                                   QuantitySampleExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0,
        //                                   PriceSampleExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0) * b.BasicPrice,
        //                                   UomUnit = b.UomUnit,
        //                                   UnitName = b.UnitName,
        //                                   ProductCode = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems where aa.POSerialNumber == b.PONo select aa.ProductCode).FirstOrDefault(),
        //                                   ProductName = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems where aa.POSerialNumber == b.PONo select aa.ProductName).FirstOrDefault(),
        //                                   EndbalanceQty = 0
        //                               };
        //        var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
        //        var querySum = Query.ToList()
        //            .GroupBy(x => new { x.PONo, x.UnitName, x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
        //            {
        //                pono = key.PONo,
        //                begining = group.Sum(s => s.BeginingbalanceQty),
        //                beginingPrice = group.Sum(s => s.BeginingbalancePrice),
        //                expendunit = group.Sum(s => s.QuantityUnitExpend),
        //                expendunitPrice = group.Sum(s => s.PriceUnitExpend),
        //                expendsample = group.Sum(s => s.QuantitySampleExpend),
        //                expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
        //                expendother = group.Sum(s => s.QuantityOtherExpend),
        //                expendotherPrice = group.Sum(s => s.PriceOtherExpend),
        //                expendlocal = group.Sum(s => s.QuantityLocalExpend),
        //                expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
        //                receipt = group.Sum(s => s.QuantityReceipt),
        //                priceReceipt = group.Sum(s => s.PriceReceipt),
        //                basicprice = group.Sum(s => s.BasicPrice),
        //                uomunit = key.UomUnit,
        //                unit = key.UnitName,
        //                productCode = key.ProductCode,
        //                productName = key.ProductName
        //            }).OrderBy(s => s.pono);

        //        foreach (var data in querySum)
        //        {

        //            GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //            {
        //                PONo = data.pono,
        //                BeginingbalanceQty = data.begining,
        //                BeginingbalancePrice = data.beginingPrice,
        //                QuantityReceipt = data.receipt,
        //                PriceReceipt = data.priceReceipt,
        //                BasicPrice = data.basicprice,
        //                QuantityUnitExpend = data.expendunit,
        //                PriceUnitExpend = data.expendunitPrice,
        //                QuantityLocalExpend = data.expendlocal,
        //                PriceLocalExpend = data.expendlocalPrice,
        //                QuantityOtherExpend = data.expendother,
        //                PriceOtherExpend = data.expendotherPrice,
        //                QuantitySampleExpend = data.expendsample,
        //                PriceSampleExpend = data.expendsamplePrice,
        //                UnitName = data.unit,
        //                UomUnit = data.uomunit,
        //                ProductCode = data.productCode,
        //                ProductName = data.productName,
        //                EndbalanceQty = data.begining + data.receipt - data.expendunit - data.expendsample - data.expendother - data.expendlocal,
        //                EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendunitPrice - data.expendsamplePrice - data.expendotherPrice - data.expendlocalPrice
        //            };
        //            GarmentLeftoverWarehouseFlowStockMonitoringViewModel.Add(garmentLeftover);
        //        }
        //    }

        //    var queryGroup = GarmentLeftoverWarehouseFlowStockMonitoringViewModel.GroupBy(x => new
        //    {
        //        x.ProductCode,
        //        x.ProductName,
        //        x.UomUnit,
        //        x.UnitName
        //    }, (key, group) => new
        //    {
        //        productcode = key.ProductCode,
        //        productname = key.ProductName,
        //        uomunit = key.UomUnit,
        //        unitname = key.UnitName,
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
        //    });
        //    var queryGroupTotal = GarmentLeftoverWarehouseFlowStockMonitoringViewModel.GroupBy(x => new
        //    {
        //        x.UnitName
        //    }, (key, group) => new
        //    {
        //        productcode = "",
        //        productname = "",
        //        uomunit = "",
        //        unitname = key.UnitName + " - TOTAL ",
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
        //    });
        //    var query = queryGroup.Union(queryGroupTotal).OrderBy(s => s.unitname);
        //    List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> stockMonitoringViewModels = new List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>();
        //    foreach (var data in query)
        //    {

        //        GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
        //        {

        //            BeginingbalanceQty = data.begining,
        //            BeginingbalancePrice = data.beginingPrice,
        //            QuantityReceipt = data.receipt,
        //            PriceReceipt = data.priceReceipt,
        //            BasicPrice = data.basicprice,
        //            QuantityUnitExpend = data.expendunit,
        //            PriceUnitExpend = data.expendunitPrice,
        //            QuantityLocalExpend = data.expendlocal,
        //            PriceLocalExpend = data.expendlocalPrice,
        //            QuantityOtherExpend = data.expendother,
        //            PriceOtherExpend = data.expendotherPrice,
        //            QuantitySampleExpend = data.expendsample,
        //            PriceSampleExpend = data.expendsamplePrice,
        //            UnitName = data.unitname,
        //            UomUnit = data.uomunit,
        //            ProductCode = data.productcode,
        //            ProductName = data.productname,
        //            EndbalanceQty = data.begining + data.receipt - data.expendunit - data.expendsample - data.expendother - data.expendlocal,
        //            EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendunitPrice - data.expendsamplePrice - data.expendotherPrice - data.expendlocalPrice
        //        };
        //        stockMonitoringViewModels.Add(garmentLeftover);
        //    }
        //    return stockMonitoringViewModels.AsQueryable();
        //}
    }
}
