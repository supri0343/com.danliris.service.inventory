using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseDetailReportService : IGarmentLeftoverWarehouseDetailReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseDetailReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseDetailReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }



        public MemoryStream GenerateExcelDetail(string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo,category, offset);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "SAT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 1A", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 1A2", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 1B", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 1B2", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 2A", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 2A1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 2B", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 2B1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 2C", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONFEKSI 2C1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH1", DataType = typeof(String) });
            var stream = new MemoryStream();

            int counter = 0;
            result.Rows.Add("",
                    "", "QTY", "RP", "QTY", "RP", "QTY", "RP", "QTY", "RP", "QTY", "RP", "QTY", "RP");
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("","", 0, 0, 0, 0, 0, 0, 0,0,0,0,0,0); // to allow column name to be generated properly for empty data as template
            else
            {

                foreach (var item in Query)
                {
                    counter++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.Description, item.Uom, item.Unit1aQty, item.Unit1aPrice, item.Unit1bQty, item.Unit1bPrice, item.Unit2aQty,item.Unit2aPrice,item.Unit2bQty, item.Unit2bPrice, item.Unit2cQty,item.Unit2cPrice,item.TotalQty,item.TotalPrice);
                }
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                    worksheet.Cells["A1"].LoadFromDataTable(result, true);
                    worksheet.Cells["C" + 1 + ":D" + 1 + ""].Merge = true;
                    worksheet.Cells["E" + 1 + ":F" + 1 + ""].Merge = true;
                    worksheet.Cells["G" + 1 + ":H" + 1 + ""].Merge = true;
                    worksheet.Cells["I" + 1 + ":J" + 1 + ""].Merge = true;
                    worksheet.Cells["K" + 1 + ":L" + 1 + ""].Merge = true;
                    worksheet.Cells["M" + 1 + ":N" + 1 + ""].Merge = true;

                    worksheet.Cells["A" + 1 + ":N" + 2 + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + 1 + ":N" + 3 + ""].Style.Font.Bold = true;
                    worksheet.Cells["A" + 1 + ":N" + (counter + 2) + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + 1 + ":N" + (counter + 2) + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + 1 + ":N" + (counter + 2) + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + 1 + ":N" + (counter + 2) + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    for (int i = 1; i < counter + 3; i++)
                    {
                        if (worksheet.Cells["A" + i].Value != null)
                        {
                            string _val = worksheet.Cells["A" + i].Value.ToString();

                            if (_val.Contains("TOTAL") || _val.Contains("SALDO AKHIR"))
                            {

                                worksheet.Cells["A" + i + ":N" + i + ""].Style.Font.Bold = true;

                            }
                            if (_val == "PENERIMAAN" || _val =="PENGELUARAN")
                            {
                                worksheet.Cells["A" + i + ":N" + i + ""].Merge = true;
                                worksheet.Cells["A" + i + ":N" + i + ""].Style.Font.Bold = true;
                            }
                        }
                    }
                    
                    foreach (var cell in worksheet.Cells["C" + 3 + ":N" + (counter + 2) + ""])
                    {
                        cell.Value = Convert.ToDecimal(cell.Value);
                        cell.Style.Numberformat.Format = "#,##0.00";
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    worksheet.Cells["A" + 1 + ":N" + (counter + 2) + ""].AutoFitColumns();

                    package.SaveAs(stream);

                }

            }

            return stream;
        }
        public Tuple<List<GarmentLeftoverWarehouseDetailReportViewModel>, int> GetMonitoringDetail(string category, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, category, offset);

            Pageable<GarmentLeftoverWarehouseDetailReportViewModel> pageable = new Pageable<GarmentLeftoverWarehouseDetailReportViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseDetailReportViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseDetailReportViewModel>();

            int TotalData = pageable.TotalCount;
            return Tuple.Create(Data, TotalData);
        }
        public IQueryable<GarmentLeftoverWarehouseDetailReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, string category, int offset)
        {

            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            List<GarmentLeftoverWarehouseDetailReportViewModel> reportViewModels = new List<GarmentLeftoverWarehouseDetailReportViewModel>();
            List<GarmentLeftoverWarehouseDetailReportViewModel> endBalanceViewModel = new List<GarmentLeftoverWarehouseDetailReportViewModel>();

            #region fabric
            if (category == "FABRIC")
            {


                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId

                                   select new GarmentLeftoverWarehouseDetailReportViewModel
                                   {
                                       Description = "SALDO AWAL",
                                       Unit2aQty = b.UnitCode == "C2A" ? b.Quantity : 0,
                                       Unit2aPrice = b.UnitCode == "C2A" ? b.Quantity * b.BasicPrice : 0,
                                       Unit2bQty = b.UnitCode == "C2B" ? b.Quantity : 0,
                                       Unit2bPrice = b.UnitCode == "C2B" ? b.Quantity * b.BasicPrice : 0,
                                       Unit2cQty = b.UnitCode == "C2C" ? b.Quantity : 0,
                                       Unit2cPrice = b.UnitCode == "C2C" ? b.Quantity * b.BasicPrice : 0,
                                       Unit1aQty = b.UnitCode == "C1A" ? b.Quantity : 0,
                                       Unit1aPrice = b.UnitCode == "C1A" ? b.Quantity * b.BasicPrice : 0,
                                       Unit1bQty = b.UnitCode == "C1B" ? b.Quantity : 0,
                                       Unit1bPrice = b.UnitCode == "C1B" ? b.Quantity * b.BasicPrice : 0,
                                   };
                var QueryReceiptBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                                     where data._IsDeleted == false
                                                && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date

                                                     select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                          join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                          select new GarmentLeftoverWarehouseDetailReportViewModel
                                          {
                                              Description = "SALDO AWAL",
                                              Unit2aQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0,
                                              Unit2aPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit2bQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0,
                                              Unit2bPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit2cQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0,
                                              Unit2cPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit1aQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0,
                                              Unit1aPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit1bQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0,
                                              Unit1bPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                          };
                var QueryExpenditureBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                         where data._IsDeleted == false
                                                    && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                         select new {  data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                              join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                         select new {expend.UnitCode, expend.BasicPrice, expend.UomUnit, expend.PONo, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                              select new GarmentLeftoverWarehouseDetailReportViewModel
                                              {
                                                  Description = "SALDO AWAL",
                                                  Unit2aQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2A" ? -b.Quantity : 0,
                                                  Unit2aPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2A" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit2bQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2B" ? -b.Quantity : 0,
                                                  Unit2bPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2B" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit2cQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2C" ? -b.Quantity : 0,
                                                  Unit2cPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2C" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit1aQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1A" ? -b.Quantity : 0,
                                                  Unit1aPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1A" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit1bQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1B" ? -b.Quantity : 0,
                                                  Unit1bPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1B" ? -b.Quantity : 0) * b.BasicPrice,

                                              };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date

                                              select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                   select new GarmentLeftoverWarehouseDetailReportViewModel
                                   {
                                       Description = "TOTAL PENERIMAAN",
                                       Unit2aQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0,
                                       Unit2aPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit2bQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0,
                                       Unit2bPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit2cQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0,
                                       Unit2cPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit1aQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0,
                                       Unit1aPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit1bQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0,
                                       Unit1bPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                   };

                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new {  data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                  select new { expend.UnitCode,expend.BasicPrice, expend.UomUnit, expend.PONo, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseDetailReportViewModel
                                       {
                                           Description = "U/" + a.ExpenditureDestination,
                                           Unit2aQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2A" ? b.Quantity : 0,
                                           Unit2aPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit2bQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2B" ? b.Quantity : 0,
                                           Unit2bPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit2cQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2C" ? b.Quantity : 0,
                                           Unit2cPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit1aQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1A" ? b.Quantity : 0,
                                           Unit1aPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit1bQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1B" ? b.Quantity : 0,
                                           Unit1bPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                       };
                var Query = QueryBalance.Union(QueryReceiptBalance).Union(QueryExpenditureBalance);
                var querySumBalance = Query.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var querySumReceipt = QueryReceipt.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var querySumExpenditure = QueryExpenditure.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var queryExpenditureTOTAL = QueryExpenditure.ToList()
                    .GroupBy(x => new { x.Uom }, (key, group) => new
                    {
                        description = "TOTAL PENGELUARAN",
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                foreach (var data in querySumBalance)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "MT",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                GarmentLeftoverWarehouseDetailReportViewModel receiptheader = new GarmentLeftoverWarehouseDetailReportViewModel
                {
                    Description = "PENERIMAAN",
                    Unit2aQty = 0,
                    Unit2aPrice = 0,
                    Unit2bQty = 0,
                    Unit2bPrice = 0,
                    Unit2cQty = 0,
                    Unit2cPrice = 0,
                    Unit1aQty = 0,
                    Unit1aPrice = 0,
                    Unit1bQty = 0,
                    Unit1bPrice = 0,
                    Uom = "MT"
                };
                reportViewModels.Add(receiptheader);
                foreach (var data in querySumReceipt)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "MT",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                GarmentLeftoverWarehouseDetailReportViewModel expendHeader = new GarmentLeftoverWarehouseDetailReportViewModel
                {
                    Description = "PENGELUARAN",
                    Unit2aQty = 0,
                    Unit2aPrice = 0,
                    Unit2bQty = 0,
                    Unit2bPrice = 0,
                    Unit2cQty = 0,
                    Unit2cPrice = 0,
                    Unit1aQty = 0,
                    Unit1aPrice = 0,
                    Unit1bQty = 0,
                    Unit1bPrice = 0,
                    Uom = "MT"
                };
                reportViewModels.Add(expendHeader);
                foreach (var data in querySumExpenditure)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "MT",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                foreach (var data in queryExpenditureTOTAL)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "MT",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                foreach (var data in reportViewModels.Where(s => s.Description != "PENGELUARAN" && !s.Description.Contains("U/")))
                {
                    if (data.Description == "SALDO AWAL" || data.Description == "TOTAL PENERIMAAN")
                    {
                        GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                        {
                            Description = "SALDO AKHIR",
                            Unit2aQty = data.Unit2aQty,
                            Unit2aPrice = data.Unit2aPrice,
                            Unit2bQty = data.Unit2bQty,
                            Unit2bPrice = data.Unit2bPrice,
                            Unit2cQty = data.Unit2cQty,
                            Unit2cPrice = data.Unit2cPrice,
                            Unit1aQty = data.Unit1aQty,
                            Unit1aPrice = data.Unit1aPrice,
                            Unit1bQty = data.Unit1bQty,
                            Unit1bPrice = data.Unit1bPrice,
                            Uom = "MT",
                            TotalQty = data.TotalQty,
                            TotalPrice = data.TotalPrice

                        };
                        endBalanceViewModel.Add(garmentLeftover);
                    }
                    else
                    {
                        GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                        {
                            Description = "SALDO AKHIR",
                            Unit2aQty = -data.Unit2aQty,
                            Unit2aPrice = -data.Unit2aPrice,
                            Unit2bQty = -data.Unit2bQty,
                            Unit2bPrice = -data.Unit2bPrice,
                            Unit2cQty = -data.Unit2cQty,
                            Unit2cPrice = -data.Unit2cPrice,
                            Unit1aQty = -data.Unit1aQty,
                            Unit1aPrice = -data.Unit1aPrice,
                            Unit1bQty = -data.Unit1bQty,
                            Unit1bPrice = -data.Unit1bPrice,
                            Uom = "MT",
                            TotalQty = -data.TotalQty,
                            TotalPrice = -data.TotalPrice
                        };
                        endBalanceViewModel.Add(garmentLeftover);
                    }
                }
                var queryENDBALANCE = endBalanceViewModel.ToList()
                                   .GroupBy(x => new { x.Uom, x.Description }, (key, group) => new
                                   {
                                       description = key.Description,
                                       unit2aqty = group.Sum(s => s.Unit2aQty),
                                       unit2aprice = group.Sum(s => s.Unit2aPrice),
                                       unit2bqty = group.Sum(s => s.Unit2bQty),
                                       unit2bprice = group.Sum(s => s.Unit2bPrice),
                                       unit2cqty = group.Sum(s => s.Unit2cQty),
                                       unit2cprice = group.Sum(s => s.Unit2cPrice),
                                       unit1aqty = group.Sum(s => s.Unit1aQty),
                                       unit1aprice = group.Sum(s => s.Unit1aPrice),
                                       unit1bqty = group.Sum(s => s.Unit1bQty),
                                       unit1bprice = group.Sum(s => s.Unit1bPrice)

                                   }).OrderByDescending(s => s.description);
                foreach (var data in queryENDBALANCE)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "MT",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }

            }
            #endregion
            #region BArangJadi
            else if (category == "BARANG JADI")
            {

                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "BARANG JADI"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId

                                   select new GarmentLeftoverWarehouseDetailReportViewModel
                                   {
                                       Description = "SALDO AWAL",
                                       Unit2aQty = b.UnitCode == "C2A" ? b.Quantity : 0,
                                       Unit2aPrice = b.UnitCode == "C2A" ? b.Quantity * b.BasicPrice : 0,
                                       Unit2bQty = b.UnitCode == "C2B" ? b.Quantity : 0,
                                       Unit2bPrice = b.UnitCode == "C2B" ? b.Quantity * b.BasicPrice : 0,
                                       Unit2cQty = b.UnitCode == "C2C" ? b.Quantity : 0,
                                       Unit2cPrice = b.UnitCode == "C2C" ? b.Quantity * b.BasicPrice : 0,
                                       Unit1aQty = b.UnitCode == "C1A" ? b.Quantity : 0,
                                       Unit1aPrice = b.UnitCode == "C1A" ? b.Quantity * b.BasicPrice : 0,
                                       Unit1bQty = b.UnitCode == "C1B" ? b.Quantity : 0,
                                       Unit1bPrice = b.UnitCode == "C1B" ? b.Quantity * b.BasicPrice : 0,
                                   };
                var QueryReceiptBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                                     where data._IsDeleted == false
                                                && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date

                                                     select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                          join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                          select new GarmentLeftoverWarehouseDetailReportViewModel
                                          {
                                              Description = "SALDO AWAL",
                                              Unit2aQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0,
                                              Unit2aPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit2bQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0,
                                              Unit2bPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit2cQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0,
                                              Unit2cPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit1aQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0,
                                              Unit1aPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit1bQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0,
                                              Unit1bPrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                          };
                var QueryExpenditureBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                         where data._IsDeleted == false
                                                               && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                         select new { data.ExpenditureDate, data.Id, data.ExpenditureTo })
                                              join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                         select new { expend.BasicPrice, expend.FinishedGoodExpenditureId, expend.UnitCode, expend.ExpenditureQuantity, expend.LeftoverComodityCode, expend.RONo, expend.LeftoverComodityName }
                                                         ) on a.Id equals b.FinishedGoodExpenditureId
                                              select new GarmentLeftoverWarehouseDetailReportViewModel
                                              {
                                                  Description = "SALDO AWAL",
                                                  Unit2aQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2A" ? -b.ExpenditureQuantity : 0,
                                                  Unit2aPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2A" ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
                                                  Unit2bQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2B" ? -b.ExpenditureQuantity : 0,
                                                  Unit2bPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2B" ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
                                                  Unit2cQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2C" ? -b.ExpenditureQuantity : 0,
                                                  Unit2cPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2C" ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
                                                  Unit1aQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1A" ? -b.ExpenditureQuantity : 0,
                                                  Unit1aPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1A" ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
                                                  Unit1bQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1B" ? -b.ExpenditureQuantity : 0,
                                                  Unit1bPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1B" ? -b.ExpenditureQuantity : 0) * b.BasicPrice,

                                              };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date

                                              select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                   select new GarmentLeftoverWarehouseDetailReportViewModel
                                   {
                                       Description = "TOTAL PENERIMAAN",
                                       Unit2aQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0,
                                       Unit2aPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit2bQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0,
                                       Unit2bPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit2cQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0,
                                       Unit2cPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit1aQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0,
                                       Unit1aPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit1bQty = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0,
                                       Unit1bPrice = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && a.UnitFromCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                   };

                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                  where data._IsDeleted == false
                                                        && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                  select new { data.ExpenditureDate, data.Id, data.ExpenditureTo })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                  select new { expend.BasicPrice, expend.FinishedGoodExpenditureId, expend.UnitCode, expend.ExpenditureQuantity, expend.LeftoverComodityCode, expend.RONo, expend.LeftoverComodityName }
                                                  ) on a.Id equals b.FinishedGoodExpenditureId
                                       select new GarmentLeftoverWarehouseDetailReportViewModel
                                       {
                                           Description = "U/" + a.ExpenditureTo,
                                           Unit2aQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2A" ? b.ExpenditureQuantity : 0,
                                           Unit2aPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2A" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           Unit2bQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2B" ? b.ExpenditureQuantity : 0,
                                           Unit2bPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2B" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           Unit2cQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2C" ? b.ExpenditureQuantity : 0,
                                           Unit2cPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2C" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           Unit1aQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1A" ? b.ExpenditureQuantity : 0,
                                           Unit1aPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1A" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           Unit1bQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1B" ? b.ExpenditureQuantity : 0,
                                           Unit1bPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1B" ? b.ExpenditureQuantity : 0) * b.BasicPrice,

                                       };
                var Query = QueryBalance.Union(QueryReceiptBalance).Union(QueryExpenditureBalance);
                var querySumBalance = Query.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var querySumReceipt = QueryReceipt.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var querySumExpenditure = QueryExpenditure.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var queryExpenditureTOTAL = QueryExpenditure.ToList()
                    .GroupBy(x => new { x.Uom }, (key, group) => new
                    {
                        description = "TOTAL PENGELUARAN",
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                foreach (var data in querySumBalance)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "PCS",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                GarmentLeftoverWarehouseDetailReportViewModel receiptheader = new GarmentLeftoverWarehouseDetailReportViewModel
                {
                    Description = "PENERIMAAN",
                    Unit2aQty = 0,
                    Unit2aPrice = 0,
                    Unit2bQty = 0,
                    Unit2bPrice = 0,
                    Unit2cQty = 0,
                    Unit2cPrice = 0,
                    Unit1aQty = 0,
                    Unit1aPrice = 0,
                    Unit1bQty = 0,
                    Unit1bPrice = 0,
                    Uom = "PCS"
                };
                reportViewModels.Add(receiptheader);
                foreach (var data in querySumReceipt)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "PCS",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                GarmentLeftoverWarehouseDetailReportViewModel expendHeader = new GarmentLeftoverWarehouseDetailReportViewModel
                {
                    Description = "PENGELUARAN",
                    Unit2aQty = 0,
                    Unit2aPrice = 0,
                    Unit2bQty = 0,
                    Unit2bPrice = 0,
                    Unit2cQty = 0,
                    Unit2cPrice = 0,
                    Unit1aQty = 0,
                    Unit1aPrice = 0,
                    Unit1bQty = 0,
                    Unit1bPrice = 0,
                    Uom = "PCS"
                };
                reportViewModels.Add(expendHeader);
                foreach (var data in querySumExpenditure)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "PCS",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                foreach (var data in queryExpenditureTOTAL)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "PCS",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                foreach (var data in reportViewModels.Where(s => s.Description != "PENGELUARAN" && !s.Description.Contains("U/")))
                {
                    if (data.Description == "SALDO AWAL" || data.Description == "TOTAL PENERIMAAN")
                    {
                        GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                        {
                            Description = "SALDO AKHIR",
                            Unit2aQty = data.Unit2aQty,
                            Unit2aPrice = data.Unit2aPrice,
                            Unit2bQty = data.Unit2bQty,
                            Unit2bPrice = data.Unit2bPrice,
                            Unit2cQty = data.Unit2cQty,
                            Unit2cPrice = data.Unit2cPrice,
                            Unit1aQty = data.Unit1aQty,
                            Unit1aPrice = data.Unit1aPrice,
                            Unit1bQty = data.Unit1bQty,
                            Unit1bPrice = data.Unit1bPrice,
                            Uom = "PCS",
                            TotalQty = data.TotalQty,
                            TotalPrice = data.TotalPrice

                        };
                        endBalanceViewModel.Add(garmentLeftover);
                    }
                    else
                    {
                        GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                        {
                            Description = "SALDO AKHIR",
                            Unit2aQty = -data.Unit2aQty,
                            Unit2aPrice = -data.Unit2aPrice,
                            Unit2bQty = -data.Unit2bQty,
                            Unit2bPrice = -data.Unit2bPrice,
                            Unit2cQty = -data.Unit2cQty,
                            Unit2cPrice = -data.Unit2cPrice,
                            Unit1aQty = -data.Unit1aQty,
                            Unit1aPrice = -data.Unit1aPrice,
                            Unit1bQty = -data.Unit1bQty,
                            Unit1bPrice = -data.Unit1bPrice,
                            Uom = "PCS",
                            TotalQty = -data.TotalQty,
                            TotalPrice = -data.TotalPrice
                        };
                        endBalanceViewModel.Add(garmentLeftover);
                    }
                }
                var queryENDBALANCE = endBalanceViewModel.ToList()
                                   .GroupBy(x => new { x.Uom, x.Description }, (key, group) => new
                                   {
                                       description = key.Description,
                                       unit2aqty = group.Sum(s => s.Unit2aQty),
                                       unit2aprice = group.Sum(s => s.Unit2aPrice),
                                       unit2bqty = group.Sum(s => s.Unit2bQty),
                                       unit2bprice = group.Sum(s => s.Unit2bPrice),
                                       unit2cqty = group.Sum(s => s.Unit2cQty),
                                       unit2cprice = group.Sum(s => s.Unit2cPrice),
                                       unit1aqty = group.Sum(s => s.Unit1aQty),
                                       unit1aprice = group.Sum(s => s.Unit1aPrice),
                                       unit1bqty = group.Sum(s => s.Unit1bQty),
                                       unit1bprice = group.Sum(s => s.Unit1bPrice)

                                   }).OrderByDescending(s => s.description);
                foreach (var data in queryENDBALANCE)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "PCS",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
            }
            #endregion
            else
            {
                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "ACCESSORIES"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId

                                   select new GarmentLeftoverWarehouseDetailReportViewModel
                                   {
                                       Description = "SALDO AWAL",
                                       Unit2aQty = b.UnitCode == "C2A" ? b.Quantity : 0,
                                       Unit2aPrice = b.UnitCode == "C2A" ? b.Quantity * b.BasicPrice : 0,
                                       Unit2bQty = b.UnitCode == "C2B" ? b.Quantity : 0,
                                       Unit2bPrice = b.UnitCode == "C2B" ? b.Quantity * b.BasicPrice : 0,
                                       Unit2cQty = b.UnitCode == "C2C" ? b.Quantity : 0,
                                       Unit2cPrice = b.UnitCode == "C2C" ? b.Quantity * b.BasicPrice : 0,
                                       Unit1aQty = b.UnitCode == "C1A" ? b.Quantity : 0,
                                       Unit1aPrice = b.UnitCode == "C1A" ? b.Quantity * b.BasicPrice : 0,
                                       Unit1bQty = b.UnitCode == "C1B" ? b.Quantity : 0,
                                       Unit1bPrice = b.UnitCode == "C1B" ? b.Quantity * b.BasicPrice : 0,
                                   };
                var QueryReceiptBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                                     where data._IsDeleted == false
                                                && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date

                                                     select new { data.RequestUnitCode, data.StorageReceiveDate, data.Id })
                                          join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                          select new GarmentLeftoverWarehouseDetailReportViewModel
                                          {
                                              Description = "SALDO AWAL",
                                              Unit2aQty = a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C2A" ? b.Quantity : 0,
                                              Unit2aPrice = (a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit2bQty = a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C2B" ? b.Quantity : 0,
                                              Unit2bPrice = (a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit2cQty = a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C2C" ? b.Quantity : 0,
                                              Unit2cPrice = (a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit1aQty = a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C1A" ? b.Quantity : 0,
                                              Unit1aPrice = (a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                              Unit1bQty = a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C1B" ? b.Quantity : 0,
                                              Unit1bPrice = (a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date && a.RequestUnitCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                          };
                var QueryExpenditureBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                         where data._IsDeleted == false
                                                    && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                         select new { data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                              join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems

                                                         select new { expend.BasicPrice, expend.ExpenditureId, expend.UomUnit, expend.UnitCode, expend.Quantity, expend.PONo }
                                                         ) on a.Id equals b.ExpenditureId
                                              select new GarmentLeftoverWarehouseDetailReportViewModel
                                              {
                                                  Description = "SALDO AWAL",
                                                  Unit2aQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2A" ? -b.Quantity : 0,
                                                  Unit2aPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2A" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit2bQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2B" ? -b.Quantity : 0,
                                                  Unit2bPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2B" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit2cQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2C" ? -b.Quantity : 0,
                                                  Unit2cPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C2C" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit1aQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1A" ? -b.Quantity : 0,
                                                  Unit1aPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1A" ? -b.Quantity : 0) * b.BasicPrice,
                                                  Unit1bQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1B" ? -b.Quantity : 0,
                                                  Unit1bPrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date && b.UnitCode == "C1B" ? -b.Quantity : 0) * b.BasicPrice,

                                              };

                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                              where data._IsDeleted == false
                                         && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date

                                              select new { data.RequestUnitCode, data.StorageReceiveDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                   select new GarmentLeftoverWarehouseDetailReportViewModel
                                   {
                                       Description = "TOTAL PENERIMAAN",
                                       Unit2aQty = a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C2A" ? b.Quantity : 0,
                                       Unit2aPrice = (a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit2bQty = a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C2B" ? b.Quantity : 0,
                                       Unit2bPrice = (a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit2cQty = a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C2C" ? b.Quantity : 0,
                                       Unit2cPrice = (a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit1aQty = a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C1A" ? b.Quantity : 0,
                                       Unit1aPrice = (a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                       Unit1bQty = a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C1B" ? b.Quantity : 0,
                                       Unit1bPrice = (a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && a.RequestUnitCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new { data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems

                                                  select new { expend.BasicPrice, expend.ExpenditureId, expend.UomUnit, expend.UnitCode, expend.Quantity, expend.PONo }
                                                  ) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseDetailReportViewModel
                                       {
                                           Description = "/U" + a.ExpenditureDestination,
                                           Unit2aQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2A" ? b.Quantity : 0,
                                           Unit2aPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2A" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit2bQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2B" ? b.Quantity : 0,
                                           Unit2bPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2B" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit2cQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2C" ? b.Quantity : 0,
                                           Unit2cPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C2C" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit1aQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1A" ? b.Quantity : 0,
                                           Unit1aPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1A" ? b.Quantity : 0) * b.BasicPrice,
                                           Unit1bQty = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1B" ? b.Quantity : 0,
                                           Unit1bPrice = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && b.UnitCode == "C1B" ? b.Quantity : 0) * b.BasicPrice,

                                       };
                var Query = QueryBalance.Union(QueryReceiptBalance).Union(QueryExpenditureBalance);
                var querySumBalance = Query.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var querySumReceipt = QueryReceipt.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var querySumExpenditure = QueryExpenditure.ToList()
                    .GroupBy(x => new { x.Description }, (key, group) => new
                    {
                        description = key.Description,
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                var queryExpenditureTOTAL = QueryExpenditure.ToList()
                    .GroupBy(x => new { x.Uom }, (key, group) => new
                    {
                        description = "TOTAL PENGELUARAN",
                        unit2aqty = group.Sum(s => s.Unit2aQty),
                        unit2aprice = group.Sum(s => s.Unit2aPrice),
                        unit2bqty = group.Sum(s => s.Unit2bQty),
                        unit2bprice = group.Sum(s => s.Unit2bPrice),
                        unit2cqty = group.Sum(s => s.Unit2cQty),
                        unit2cprice = group.Sum(s => s.Unit2cPrice),
                        unit1aqty = group.Sum(s => s.Unit1aQty),
                        unit1aprice = group.Sum(s => s.Unit1aPrice),
                        unit1bqty = group.Sum(s => s.Unit1bQty),
                        unit1bprice = group.Sum(s => s.Unit1bPrice)

                    }).OrderByDescending(s => s.description);
                foreach (var data in querySumBalance)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                GarmentLeftoverWarehouseDetailReportViewModel receiptheader = new GarmentLeftoverWarehouseDetailReportViewModel
                {
                    Description = "PENERIMAAN",
                    Unit2aQty = 0,
                    Unit2aPrice = 0,
                    Unit2bQty = 0,
                    Unit2bPrice = 0,
                    Unit2cQty = 0,
                    Unit2cPrice = 0,
                    Unit1aQty = 0,
                    Unit1aPrice = 0,
                    Unit1bQty = 0,
                    Unit1bPrice = 0,
                    Uom = ""
                };
                reportViewModels.Add(receiptheader);
                foreach (var data in querySumReceipt)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                GarmentLeftoverWarehouseDetailReportViewModel expendHeader = new GarmentLeftoverWarehouseDetailReportViewModel
                {
                    Description = "PENGELUARAN",
                    Unit2aQty = 0,
                    Unit2aPrice = 0,
                    Unit2bQty = 0,
                    Unit2bPrice = 0,
                    Unit2cQty = 0,
                    Unit2cPrice = 0,
                    Unit1aQty = 0,
                    Unit1aPrice = 0,
                    Unit1bQty = 0,
                    Unit1bPrice = 0,
                    Uom = ""
                };
                reportViewModels.Add(expendHeader);
                foreach (var data in querySumExpenditure)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                foreach (var data in queryExpenditureTOTAL)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }
                foreach (var data in reportViewModels.Where(s => s.Description == "TOTAL PENGELUARAN" || s.Description == "TOTAL PENERIMAAN" || s.Description == "SALDO AWAL"))
                {
                    if (data.Description == "SALDO AWAL" || data.Description == "TOTAL PENERIMAAN")
                        {
                        GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                        {
                            Description = "SALDO AKHIR",
                            Unit2aQty = data.Unit2aQty,
                            Unit2aPrice = data.Unit2aPrice,
                            Unit2bQty = data.Unit2bQty,
                            Unit2bPrice = data.Unit2bPrice,
                            Unit2cQty = data.Unit2cQty,
                            Unit2cPrice = data.Unit2cPrice,
                            Unit1aQty = data.Unit1aQty,
                            Unit1aPrice = data.Unit1aPrice,
                            Unit1bQty = data.Unit1bQty,
                            Unit1bPrice = data.Unit1bPrice,
                            Uom = "",
                            TotalQty = data.TotalQty,
                            TotalPrice = data.TotalPrice

                        };
                        endBalanceViewModel.Add(garmentLeftover);
                    }
                    else
                    {
                        GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                        {
                            Description = "SALDO AKHIR",
                            Unit2aQty = -data.Unit2aQty,
                            Unit2aPrice = -data.Unit2aPrice,
                            Unit2bQty = -data.Unit2bQty,
                            Unit2bPrice = -data.Unit2bPrice,
                            Unit2cQty = -data.Unit2cQty,
                            Unit2cPrice = -data.Unit2cPrice,
                            Unit1aQty = -data.Unit1aQty,
                            Unit1aPrice = -data.Unit1aPrice,
                            Unit1bQty = -data.Unit1bQty,
                            Unit1bPrice = -data.Unit1bPrice,
                            Uom = "",
                            TotalQty = -data.TotalQty,
                            TotalPrice = -data.TotalPrice
                        };
                        endBalanceViewModel.Add(garmentLeftover);
                    }
                }
                var queryENDBALANCE = endBalanceViewModel.ToList()
                                   .GroupBy(x => new { x.Uom, x.Description }, (key, group) => new
                                   {
                                       description = key.Description,
                                       unit2aqty = group.Sum(s => s.Unit2aQty),
                                       unit2aprice = group.Sum(s => s.Unit2aPrice),
                                       unit2bqty = group.Sum(s => s.Unit2bQty),
                                       unit2bprice = group.Sum(s => s.Unit2bPrice),
                                       unit2cqty = group.Sum(s => s.Unit2cQty),
                                       unit2cprice = group.Sum(s => s.Unit2cPrice),
                                       unit1aqty = group.Sum(s => s.Unit1aQty),
                                       unit1aprice = group.Sum(s => s.Unit1aPrice),
                                       unit1bqty = group.Sum(s => s.Unit1bQty),
                                       unit1bprice = group.Sum(s => s.Unit1bPrice)

                                   }).OrderByDescending(s => s.description);
                foreach (var data in queryENDBALANCE)
                {
                    GarmentLeftoverWarehouseDetailReportViewModel garmentLeftover = new GarmentLeftoverWarehouseDetailReportViewModel
                    {
                        Description = data.description,
                        Unit2aQty = data.unit2aqty,
                        Unit2aPrice = data.unit2aprice,
                        Unit2bQty = data.unit2bqty,
                        Unit2bPrice = data.unit2bprice,
                        Unit2cQty = data.unit2cqty,
                        Unit2cPrice = data.unit2cprice,
                        Unit1aQty = data.unit1aqty,
                        Unit1aPrice = data.unit1aprice,
                        Unit1bQty = data.unit1bqty,
                        Unit1bPrice = data.unit1bprice,
                        Uom = "",
                        TotalQty = data.unit1aqty + data.unit1bqty + data.unit2aqty + data.unit2bqty + data.unit2cqty,
                        TotalPrice = (data.unit1aprice) + (data.unit1bprice) + ( data.unit2aprice) + ( data.unit2bprice) + (data.unit2cprice)

                    };
                    reportViewModels.Add(garmentLeftover);
                }

            }

            return reportViewModels.AsQueryable();
        }
    }
}
