using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure.Aval
{
    public class ExpenditureAvalMonitoringService : IExpenditureAvalMonitoringService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseExpenditureAvalMonitoringService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseExpenditureAval> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public ExpenditureAvalMonitoringService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseExpenditureAval>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

        }

        public int TotalCountReport { get; set; } = 0;
        private List<ExpenditureAvalMonitoringViewModel> GetMonitoringQuery(DateTime? dateFrom, DateTime? dateTo, string type, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<ExpenditureAvalMonitoringViewModel> listData = new List<ExpenditureAvalMonitoringViewModel>();

            var query = from a in DbContext.GarmentLeftoverWarehouseExpenditureAvals
                        where a.AvalType == (type == null ? a.AvalType : type)
                        && a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                        && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                        select new
                        {
                            avalId = a.Id,
                            date = a.ExpenditureDate
                        };

            TotalCountReport = query.Distinct().OrderByDescending(o => o.date).Count();
            var queryResult = size != 0 && page != 0 ? query.Distinct().OrderByDescending(o => o.date).Skip((page - 1) * size).Take(size).ToList() : query.Distinct().OrderByDescending(o => o.date).ToList();

            var join = from a in queryResult
                       join b in DbContext.GarmentLeftoverWarehouseExpenditureAvalItems on a.avalId equals b.AvalExpenditureId
                       select new
                       {
                           a.avalId,
                           itemId = b.Id,
                           a.date
                       };

            var avalIds = join.Select(s => s.avalId).Distinct().ToList();
            var avals = DbContext.GarmentLeftoverWarehouseExpenditureAvals.Where(w => avalIds.Contains(w.Id)).Select(s => new { s.Id, s.AvalExpenditureNo, s.ExpenditureDate, s.AvalType, s.BuyerName, s.OtherDescription, s.ExpenditureTo,s.LocalSalesNoteNo}).ToList();
            var itemIds = join.Select(s => s.itemId).Distinct().ToList();
            var items = DbContext.GarmentLeftoverWarehouseExpenditureAvalItems.Where(w => itemIds.Contains(w.Id)).Select(s => new { s.Id, s.ProductCode, s.ProductName, s.Quantity, s.UomUnit, s.AvalReceiptNo, s.UnitCode }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in join)
            {
                var aval = avals.FirstOrDefault(f => f.Id.Equals(item.avalId));
                var avalItem = items.FirstOrDefault(f => f.Id.Equals(item.itemId));

                ExpenditureAvalMonitoringViewModel vm = new ExpenditureAvalMonitoringViewModel();

                vm.AvalType = aval.AvalType;
                vm.ProductCode = aval.AvalType == "AVAL BAHAN PENOLONG" ? avalItem.ProductCode : "-";
                vm.Quantity = avalItem.Quantity;
                vm.ProductName = aval.AvalType == "AVAL BAHAN PENOLONG" ? avalItem.ProductName : "-";
                vm.ExpenditureDate = aval.ExpenditureDate;
                vm.AvalReceiptNo = aval.AvalType == "AVAL BAHAN PENOLONG" ? "-" : avalItem.AvalReceiptNo;
                vm.UomUnit = aval.AvalType== "AVAL BAHAN PENOLONG" ? avalItem.UomUnit : "KG";
                vm.Quantity = avalItem.Quantity;
                vm.UnitCode = avalItem.UnitCode;
                vm.ExpenditureTo = aval.ExpenditureTo;
                vm.ExpenditureNo = aval.AvalExpenditureNo;
                vm.OtherDescription = aval.ExpenditureTo == "JUAL LOKAL" ? aval.BuyerName : aval.OtherDescription;
                vm.LocalSalesNoteNo = aval.ExpenditureTo == "JUAL LOKAL" ? aval.LocalSalesNoteNo : "-";

                vm.index = i;
                if (listData.Where(a => a.ExpenditureNo == vm.ExpenditureNo).Count() == 0)
                {
                    i++;
                }
                listData.Add(vm);


            }
            if (size != 0) {
                if (page == ((listData.Count() / size) + 1) && listData.Count() != 0)
                {
                    var QtyTotal = listData.Sum(x => x.Quantity);
                    ExpenditureAvalMonitoringViewModel vm = new ExpenditureAvalMonitoringViewModel();

                    vm.AvalType = "";
                    vm.ProductCode = "";
                    vm.Quantity = QtyTotal;
                    vm.ProductName = "";
                    vm.ExpenditureDate = DateTimeOffset.MinValue;
                    vm.AvalReceiptNo = "";
                    vm.UomUnit = "";
                    vm.Quantity = QtyTotal;
                    vm.UnitCode = "";
                    vm.ExpenditureTo = "";
                    vm.ExpenditureNo = "T O T A L";
                    vm.OtherDescription = "";
                    vm.LocalSalesNoteNo = "";

                    listData.Add(vm);

                }
            }
            

            return listData;
        }

        public Tuple<List<ExpenditureAvalMonitoringViewModel>, int> GetMonitoring(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order, int offset)
        {
            var Data = GetMonitoringQuery(dateFrom, dateTo, type, offset, page, size);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }


        public Tuple<MemoryStream, string> GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string type, int offset)
        {
            var Query = GetMonitoringQuery(dateFrom, dateTo, type, offset, 0, 0);
            var data = Query.ToList();

            var QtyTotal = data.Sum(x => x.Quantity);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Keluar", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bon Keluar", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Aval", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Tujuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Jual Lokal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Penerimaan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit Asal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });

            //List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();
            int idx = 1;
            var rCount = 0;
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                //int index = 0;

                foreach (var item in Query.ToList())
                {
                    idx++;
                    if (!Rowcount.ContainsKey(item.ExpenditureNo))
                    {
                        rCount = 0;
                        var index = idx;
                        Rowcount.Add(item.ExpenditureNo, index.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.ExpenditureNo] = Rowcount[item.ExpenditureNo] + "-" + rCount.ToString();
                        var val = Rowcount[item.ExpenditureNo].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.ExpenditureNo] = val[0] + "-" + rCount.ToString();
                        }
                    }
                    string ExpenditureDate = item.ExpenditureDate == new DateTime(1970, 1, 1) ? "-" : item.ExpenditureDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
                    
                    result.Rows.Add(item.index, item.ExpenditureNo, ExpenditureDate, item.AvalType, item.ExpenditureTo, item.OtherDescription, item.LocalSalesNoteNo
                        , item.AvalReceiptNo, item.ProductCode, item.ProductName, item.UnitCode, item.Quantity, item.UomUnit);

                }

                result.Rows.Add("", "T O T A L .........", "", "", "", "", ""
                        , "", "", "", "", QtyTotal, "");
            }
            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report Pengeluaran Gudang Sisa Aval");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);


            foreach (var rowMerge in Rowcount)
            {
                var UnitrowNum = rowMerge.Value.Split("-");
                int rowNum2 = 1;
                int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
                if (UnitrowNum.Length > 1)
                {
                    rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                }
                else
                {
                    rowNum2 = Convert.ToInt32(rowNum1);
                }

                sheet.Cells[$"A{rowNum1}:A{rowNum2}"].Merge = true;
                sheet.Cells[$"A{rowNum1}:A{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A{rowNum1}:A{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells[$"B{rowNum1}:B{rowNum2}"].Merge = true;
                sheet.Cells[$"B{rowNum1}:B{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"B{rowNum1}:B{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells[$"C{rowNum1}:C{rowNum2}"].Merge = true;
                sheet.Cells[$"C{rowNum1}:C{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"C{rowNum1}:C{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells[$"D{rowNum1}:D{rowNum2}"].Merge = true;
                sheet.Cells[$"D{rowNum1}:D{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"D{rowNum1}:D{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells[$"E{rowNum1}:E{rowNum2}"].Merge = true;
                sheet.Cells[$"E{rowNum1}:E{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"E{rowNum1}:E{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells[$"F{rowNum1}:F{rowNum2}"].Merge = true;
                sheet.Cells[$"F{rowNum1}:F{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"F{rowNum1}:F{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells[$"G{rowNum1}:G{rowNum2}"].Merge = true;
                sheet.Cells[$"G{rowNum1}:G{rowNum2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"G{rowNum1}:G{rowNum2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            MemoryStream streamExcel = new MemoryStream();
            package.SaveAs(streamExcel);

            //Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);
            string fileName = string.Concat("Report Pengeluaran Gudang Sisa - AVAL ", ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }

    }
}
