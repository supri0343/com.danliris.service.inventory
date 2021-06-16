using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Data;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt.Aval
{
    public class ReceiptAvalMonitoringService : IReceiptAvalMonitoringService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptAvalMonitoringService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptAval> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public ReceiptAvalMonitoringService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptAval>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

        }

        public int TotalCountReport { get; set; } = 0;
        private List<ReceiptAvalMonitoringViewModel> GetMonitoringQuery(DateTime? dateFrom, DateTime? dateTo, string type, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<ReceiptAvalMonitoringViewModel> listData = new List<ReceiptAvalMonitoringViewModel>();

            var query = from a in DbContext.GarmentLeftoverWarehouseReceiptAvals
                       //join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.Id equals b.AvalReceiptId
                       where a.AvalType == (type == null ? a.AvalType : type)
                       && a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
                       && a.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                       select new
                       {
                           avalId = a.Id,
                          // itemId = b.Id,
                           date = a.ReceiptDate
                       };

            TotalCountReport = query.Distinct().OrderByDescending(o => o.date).Count();
            var queryResult = size != 0 && page != 0 ? query.Distinct().OrderByDescending(o => o.date).Skip((page - 1) * size).Take(size).ToList() : query.Distinct().OrderByDescending(o => o.date).ToList();

            var join = from a in queryResult
                       join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.avalId equals b.AvalReceiptId
                       select new
                       {
                           a.avalId,
                           itemId = b.Id,
                           a.date
                       };

            var avalIds = join.Select(s => s.avalId).Distinct().ToList();
            var avals = DbContext.GarmentLeftoverWarehouseReceiptAvals.Where(w => avalIds.Contains(w.Id)).Select(s => new { s.Id, s.AvalReceiptNo, s.ReceiptDate, s.UnitFromCode, s.AvalType, s.TotalAval }).ToList();
            var itemIds = join.Select(s => s.itemId).Distinct().ToList();
            var items = DbContext.GarmentLeftoverWarehouseReceiptAvalItems.Where(w => itemIds.Contains(w.Id)).Select(s => new { s.Id, s.RONo, s.ProductCode, s.ProductName, s.Quantity, s.UomUnit, s.ProductRemark, s.AvalComponentNo }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in join)
            {
                var aval = avals.FirstOrDefault(f => f.Id.Equals(item.avalId));
                var avalItem = items.FirstOrDefault(f => f.Id.Equals(item.itemId));

                ReceiptAvalMonitoringViewModel vm = new ReceiptAvalMonitoringViewModel();

                vm.AvalType = aval.AvalType;
                vm.ProductCode = aval.AvalType == "AVAL KOMPONEN" ? "-" : avalItem.ProductCode;
                vm.Quantity = avalItem.Quantity;
                vm.Remark = avalItem.ProductRemark;
                vm.ProductName = aval.AvalType == "AVAL KOMPONEN" ? "KOMPONEN" : avalItem.ProductName;
                vm.ReceiptDate = aval.ReceiptDate;
                vm.ReceiptNoteNo = aval.AvalReceiptNo;
                vm.Weight = aval.TotalAval;
                vm.Id = aval.Id;
                vm.UomUnit = avalItem.UomUnit;
                vm.Quantity = avalItem.Quantity;
                vm.RONo = aval.AvalType != "AVAL BAHAN PENOLONG" ? avalItem.RONo : "-";
                vm.AvalComponentNo = aval.AvalType != "AVAL KOMPONEN" ? "-" : avalItem.AvalComponentNo;
                vm.UnitCode = aval.UnitFromCode;
                vm.Uom = aval.AvalType != "AVAL BAHAN PENOLONG" ? "KG" : "-";

                vm.index = i;
                if (listData.Where(a => a.Id == vm.Id).Count() == 0)
                {
                    i++;
                }
                listData.Add(vm);
                

            }

            if(size !=0)
            {
                if (page == ((listData.Count() / size) + 1) && listData.Count() != 0)
                {
                    var QtyTotal = listData.Sum(x => x.Quantity);

                    var Query2 = listData.GroupBy(x => x.ReceiptNoteNo, (key, group) => new {
                        Qty = group.FirstOrDefault().Weight
                    });

                    var WeightTotal = Query2.Sum(x => x.Qty);
                    ReceiptAvalMonitoringViewModel vm = new ReceiptAvalMonitoringViewModel();

                    vm.AvalType = "";
                    vm.ProductCode = "";
                    vm.Quantity = QtyTotal;
                    vm.Remark = "";
                    vm.ProductName = "";
                    vm.ReceiptDate = DateTimeOffset.MinValue;
                    vm.ReceiptNoteNo = "T O T A L";
                    vm.Weight = WeightTotal;
                    vm.Id = 0;
                    vm.UomUnit = "";
                    vm.Quantity = QtyTotal;
                    vm.RONo = "";
                    vm.AvalComponentNo = "";
                    vm.UnitCode = "";
                    vm.Uom = "";

                    listData.Add(vm);

                }
            }
            

            return listData;
        }

        public Tuple<List<ReceiptAvalMonitoringViewModel>, int> GetMonitoring(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order, int offset)
        {
            var Data = GetMonitoringQuery(dateFrom, dateTo,type, offset, page, size);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }

        public Tuple<MemoryStream, string> GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string type, int offset)
        {
            var Query = GetMonitoringQuery(dateFrom, dateTo, type, offset, 0, 0);
            var data = Query.ToList();

            var Query2 = data.GroupBy(x => x.ReceiptNoteNo, (key, group) => new {
                Qty = group.FirstOrDefault().Weight
            });

            var QtyTotal = data.Sum(x => x.Quantity);
            var WeightTotal = Query2.Sum(x => x.Qty);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Terima", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bon Terima", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Aval", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Berat", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor RO", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Aval Komponen", DataType = typeof(string) });

            //List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();
            int idx = 1;
            var rCount = 0;
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                //int index = 0;

                foreach (var item in Query.ToList())
                {
                    idx++;
                    if (!Rowcount.ContainsKey(item.ReceiptNoteNo))
                    {
                        rCount = 0;
                        var index = idx;
                        Rowcount.Add(item.ReceiptNoteNo, index.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.ReceiptNoteNo] = Rowcount[item.ReceiptNoteNo] + "-" + rCount.ToString();
                        var val = Rowcount[item.ReceiptNoteNo].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.ReceiptNoteNo] = val[0] + "-" + rCount.ToString();
                        }
                    }
                    string ReceiptDate = item.ReceiptDate == new DateTime(1970, 1, 1) ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
                    string Weight = item.Weight == 0 ? "-" : item.Weight.ToString();
                    
                    result.Rows.Add(item.index, item.ReceiptNoteNo, ReceiptDate, item.AvalType, Weight, item.Uom, item.UnitCode
                        , item.RONo, item.ProductCode, item.ProductName, item.Remark, item.Quantity, item.UomUnit, item.AvalComponentNo);

                }

                result.Rows.Add("", "T O T A L . . . .", "", "", WeightTotal, "", ""
                        , "", "", "", "", QtyTotal, "", "");
            }
            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report Penerimaan Gudang Sisa Aval");
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
            string fileName = string.Concat("Report Penerimaan Gudang Sisa - AVAL ", ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }
    }
}
