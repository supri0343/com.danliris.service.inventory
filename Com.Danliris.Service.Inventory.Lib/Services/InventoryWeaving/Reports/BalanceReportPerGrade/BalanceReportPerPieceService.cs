using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerMonth;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Moonlay.NetCore.Lib;
using System.Linq;
using System.IO;
using System.Data;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.BalanceReportPerGrade
{
    public class BalanceReportPerPieceService : IBalanceReportPerPieceService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public BalanceReportPerPieceService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<Tuple<List<BalanceReportPerPieceViewModel>, int>> GetStockReportGreige(DateTime? dateTo, int offset, int page, int size, string Order)
        {

            List<BalanceReportPerPieceViewModel> Query = await GetStockQuery(dateTo, offset);
            //Query = Query.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
            //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
            //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
            //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
            //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();

            //Query = Query.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();
            Pageable<BalanceReportPerPieceViewModel> pageable = new Pageable<BalanceReportPerPieceViewModel>(Query, page - 1, size);
            List<BalanceReportPerPieceViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }

        public async Task<List<BalanceReportPerPieceViewModel>> GetStockQuery(DateTime? dateto, int offset)
        {
            DateTime dateReport = dateto == null ? DateTime.Now : (DateTime)dateto;
            var startDate = new DateTime(dateReport.Year, dateReport.Month, 1);


            var QueryAll = (from a in DbSetMovement
                            join c in DbSetDoc on a.InventoryWeavingDocumentId equals c.Id into m
                            from b in m.DefaultIfEmpty()
                            where
                            a._IsDeleted == false
                            && b._IsDeleted == false
                            //&& a.Construction.Contains("C D    120  60  63  RF  RF  B  B")

                            select new BalanceReportPerPieceAllViewModel
                            {
                                Date = b.Date,
                                _CreatedUtc = a._CreatedUtc,
                                Construction = a.Construction,
                                MaterialName = a.MaterialName,
                                WovenType = a.WovenType,
                                Yarn1 = a.Yarn1,
                                Yarn2 = a.Yarn2,
                                Width = a.Width,
                                YarnOrigin1 = a.YarnOrigin1,
                                YarnOrigin2 = a.YarnOrigin2,
                                YarnType1 = a.YarnType1,
                                YarnType2 = a.YarnType2,
                                Area = b.BonType != null ? b.BonType : "-",
                                Quantity = a.Quantity, //!= null ? a.Quantity: 0,
                                QuantityPiece = a.QuantityPiece,
                                Piece = a.Piece,
                                Grade = a.Grade,
                                Type = a.Type



                            }


                );

            var TerimaAll = QueryAll.Where(d => d.Date.AddHours(offset).Date >= startDate.Date && d.Date.AddHours(offset).Date <= dateReport.Date && d.Type == "IN").Select(x => new BalanceReportPerPieceViewModel
            {
                Construction = x.Construction,
                MaterialName = x.MaterialName,
                WovenType = x.WovenType,
                Yarn1 = x.Yarn1,
                Yarn2 = x.Yarn2,
                Width = x.Width,
                YarnOrigin1 = x.YarnOrigin1,
                YarnOrigin2 = x.YarnOrigin2,
                YarnType1 = x.YarnType1,
                YarnType2 = x.YarnType2,
                Piece = x.Piece,
                Grade = x.Grade,
                ReceiptBalance =  x.Type == "IN" ? x.Quantity : 0,
                ReceiptPieceQuantity = x.Type == "IN" ? x.QuantityPiece : 0,
                ExpendBalance = 0,
                ExpendPieceQuantity = 0,
                BeginingBalance = 0,
                BeginingBalancePiece = 0,
                EndingBalance = 0,
                EndingBalancePiece = 0,
            });


            var Terima = TerimaAll.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Piece,
                s.Grade

            }).Select(x => new BalanceReportPerPieceViewModel
            {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Piece = x.Key.Piece,
                Grade = x.Key.Grade,
                ReceiptBalance = x.Sum( f => f.ReceiptBalance),
                ReceiptPieceQuantity = x.Sum( f => f.ReceiptPieceQuantity),
                ExpendBalance = x.Sum(f=> f.ExpendBalance),
                ExpendPieceQuantity = x.Sum(f=>f.ExpendPieceQuantity),
                BeginingBalance = 0,
                BeginingBalancePiece = 0,
                EndingBalance = 0,
                EndingBalancePiece = 0
            });

            var KeluarAll = QueryAll.Where(d => d.Date.AddHours(offset).Date >= startDate.Date && d.Date.AddHours(offset).Date <= dateReport.Date && d.Type == "OUT").Select(x => new BalanceReportPerPieceViewModel
            {
                Construction = x.Construction,
                MaterialName = x.MaterialName,
                WovenType = x.WovenType,
                Yarn1 = x.Yarn1,
                Yarn2 = x.Yarn2,
                Width = x.Width,
                YarnOrigin1 = x.YarnOrigin1,
                YarnOrigin2 = x.YarnOrigin2,
                YarnType1 = x.YarnType1,
                YarnType2 = x.YarnType2,
                Piece = x.Piece,
                Grade = x.Grade,
                ReceiptBalance = 0,
                ReceiptPieceQuantity = 0,
                ExpendBalance = x.Type == "OUT" ? x.Quantity : 0,
                ExpendPieceQuantity = x.Type == "OUT" ? x.QuantityPiece : 0,

                BeginingBalance = 0,
                BeginingBalancePiece = 0,
                EndingBalance = 0,
                EndingBalancePiece = 0
            });



            var Keluar = KeluarAll.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Piece,
                s.Grade


            }).Select(x => new BalanceReportPerPieceViewModel
            {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Piece = x.Key.Piece,
                Grade = x.Key.Grade,
                ReceiptBalance = 0,
                ReceiptPieceQuantity = 0,
                ExpendBalance = x.Sum( f => f.ExpendBalance),
                ExpendPieceQuantity = x.Sum( f=> f.ExpendPieceQuantity),
                BeginingBalance = 0,
                BeginingBalancePiece = 0,
                EndingBalance = 0,
                EndingBalancePiece = 0

            });


            var SaldoAllAwal = QueryAll.Where(d => d.Date.AddHours(offset).Date < startDate.Date).GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Type,
                s.Piece,
                s.Grade

            }).Select(x => new BalanceReportPerPieceAllViewModel
            {
                _CreatedUtc = x.FirstOrDefault()._CreatedUtc,
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Piece = x.Key.Piece,
                Grade = x.Key.Grade,
                Area = x.FirstOrDefault().Area,
                Quantity = x.Sum(d => d.Quantity),
                QuantityPiece = x.Sum( d => d.QuantityPiece),
                Type = x.Key.Type


            });

            var SaldoAllAkhir = QueryAll.Where(d => d.Date.AddHours(offset).Date <= dateReport.Date).GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Type,
                s.Piece,
                s.Grade

            }).Select(x => new BalanceReportPerPieceAllViewModel
            {
                _CreatedUtc = x.FirstOrDefault()._CreatedUtc,
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Area = x.FirstOrDefault().Area,
                Quantity = x.Sum(d => d.Quantity),
                QuantityPiece = x.Sum(d=>d.QuantityPiece),
                Piece = x.Key.Piece,
                Grade = x.Key.Grade,
                Type = x.Key.Type


            });

            var SaldoAwal = SaldoAllAwal.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Piece,
                s.Grade

            }).Select(x => new BalanceReportPerPieceViewModel
            {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Piece = x.Key.Piece,
                Grade =x.Key.Grade,
                ReceiptBalance = 0,
                ReceiptPieceQuantity = 0,
                ExpendBalance = 0,
                ExpendPieceQuantity = 0,
                BeginingBalance = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").Quantity - x.FirstOrDefault(d => d.Type == "OUT").Quantity :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").Quantity : 0,
                BeginingBalancePiece = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece - x.FirstOrDefault(d => d.Type == "OUT").QuantityPiece :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece : 0,
                EndingBalance = 0,
                EndingBalancePiece =0 
            });



            var SaldoAkhir = SaldoAllAkhir.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Piece,
                s.Grade

            }).Select(x => new BalanceReportPerPieceViewModel
            {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Piece = x.Key.Piece,
                Grade = x.Key.Grade,
                ReceiptBalance = 0,
                ReceiptPieceQuantity = 0,
                ExpendBalance = 0,
                ExpendPieceQuantity = 0,
                BeginingBalance = 0,
                BeginingBalancePiece = 0,
                EndingBalance = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").Quantity - x.FirstOrDefault(d => d.Type == "OUT").Quantity :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").Quantity : 0,
                EndingBalancePiece = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece - x.FirstOrDefault(d => d.Type == "OUT").QuantityPiece :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece : 0,

            });


            // var Stock = Terima.Concat(Keluar).Concat(SaldoAwal).Concat(SaldoAkhir);

            //var cTerima = Terima.Count();
            //var cKeluar = Keluar.Count();
            //var cSaldoAwal = SaldoAwal.Count();
            //var cSaldoAkhir = SaldoAkhir.Count();

            var Stock = SaldoAwal.Concat(SaldoAkhir).Concat(Terima).Concat(Keluar).AsEnumerable();

            var DataStock = Stock.GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Piece,
                s.Grade
            }, (key, data) => new BalanceReportPerPieceViewModel
            {
                Construction = data.FirstOrDefault().Construction,
                MaterialName = key.MaterialName,
                WovenType = key.WovenType,
                Yarn1 = key.Yarn1,
                Yarn2 = key.Yarn2,
                Width = key.Width,
                YarnOrigin1 = key.YarnOrigin1,
                YarnOrigin2 = key.YarnOrigin2,
                YarnType1 = key.YarnType1,
                YarnType2 = key.YarnType2,
                Piece = key.Piece,
                Grade = key.Grade,
                ReceiptBalance = data.Sum( d => d.ReceiptBalance),
                ReceiptPieceQuantity = data.Sum( d => d.ReceiptPieceQuantity),
                ExpendBalance = data.Sum( d => d.ExpendBalance),
                ExpendPieceQuantity = data.Sum( d=> d.ExpendPieceQuantity),
                BeginingBalance = data.Sum(d => d.BeginingBalance),
                BeginingBalancePiece = data.Sum( d => d.BeginingBalancePiece),
                EndingBalance = data.Sum(d => d.EndingBalance),
                EndingBalancePiece = data.Sum( d => d.EndingBalancePiece)
            });


            List<BalanceReportPerPieceViewModel> stockReportViewModels = new List<BalanceReportPerPieceViewModel>();
            int index = 1;
            foreach (var i in DataStock)
            {

                stockReportViewModels.Add(new BalanceReportPerPieceViewModel
                {
                    Nomor = index++,
                    Construction = i.Construction,
                    MaterialName = i.MaterialName,
                    WovenType = i.WovenType,
                    Yarn1 = i.Yarn1,
                    Yarn2 = i.Yarn2,
                    Width = i.Width,
                    YarnOrigin1 = i.YarnOrigin1,
                    YarnOrigin2 = i.YarnOrigin2,
                    YarnType1 = i.YarnType1,
                    YarnType2 = i.YarnType2,
                    Piece = i.Piece == "1" ? "BESAR" : i.Piece == "2" ? "KECIL" : "POTONGAN",
                    Grade = i.Grade,
                    ReceiptBalance = i.ReceiptBalance,
                    ReceiptPieceQuantity = i.ReceiptPieceQuantity,
                    ExpendBalance = i.ExpendBalance,
                    ExpendPieceQuantity = i.ExpendPieceQuantity,
                    BeginingBalance = i.BeginingBalance,
                    BeginingBalancePiece = i.BeginingBalancePiece,
                    EndingBalance = i.EndingBalance,
                    EndingBalancePiece = i.EndingBalancePiece

                });



            }

            var total = new BalanceReportPerPieceViewModel
            {
                Construction = "TOTAL",
                MaterialName = "-",
                WovenType = "-",
                Yarn1 = "-",
                Yarn2 = "-",
                Width = "-",
                YarnOrigin1 = "-",
                YarnOrigin2 = "-",
                YarnType1 = "-",
                YarnType2 = "-",
                Piece = "",
                Grade = "",
                ReceiptBalance = Math.Round(DataStock.Sum(X => X.ReceiptBalance), 2),
                ReceiptPieceQuantity = Math.Round(DataStock.Sum(X => X.ReceiptPieceQuantity), 2),
                ExpendBalance = Math.Round(DataStock.Sum(X => X.ExpendBalance), 2),
                ExpendPieceQuantity = Math.Round(DataStock.Sum(X => X.ExpendPieceQuantity), 2),
                BeginingBalance = Math.Round(DataStock.Sum(X => X.BeginingBalance), 2),
                BeginingBalancePiece = Math.Round(DataStock.Sum(X => X.BeginingBalancePiece), 2),
                EndingBalance = Math.Round(DataStock.Sum(X => X.EndingBalance), 2),
                EndingBalancePiece = Math.Round(DataStock.Sum(X => X.EndingBalancePiece), 2),

            };


            stockReportViewModels.Add(total);

            return stockReportViewModels;


        }

        public async Task<MemoryStream> GenerateExcel(DateTime? dateTo, int offset)
        {
            var Query = await GetStockQuery(dateTo, offset);
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            string Bulan = DateTo.ToString("MMM yyyy", new CultureInfo("id-ID"));
            string Tanggal = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

            DataTable result = new DataTable();
            var headers = new string[] { "No", "Konstruksi", "JL", "JP", "Grade", "Jenis", "Awal", "Awal1", "Masuk", "Masuk1", "Keluar", "Keluar1", "Akhir", "Akhir1"};
            var subheaders = new string[] { "Piece", "Meter", "Piece", "Meter", "Piece", "Meter", "Piece", "Meter", };


            result.Columns.Add(new DataColumn() { ColumnName = headers[0], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[1], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[2], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[3], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[4], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[5], DataType = typeof(string) });

            for (int i = 6; i < 14; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(Double) });
            }

            Query.RemoveAt(Query.Count() - 1);

            double TotBeginingBalancePiece = 0;
            double TotBeginingBalance = 0;
            double TotReceiptPieceQuantity = 0;
            double TotReceiptBalance = 0;
            double TotExpandPieceQuantity = 0;
            double TotExpandBalance = 0;
            double TotEndingBalancePiece = 0;
            double TotEndingBalance = 0;
            
            
            //double TotEndingBalance = 0;



            foreach (var item in Query)
            {
                TotBeginingBalancePiece += item.BeginingBalancePiece;
                TotBeginingBalance += item.BeginingBalance;
                TotReceiptPieceQuantity += item.ReceiptPieceQuantity;
                TotReceiptBalance += item.ReceiptBalance;
                TotExpandPieceQuantity += item.ExpendPieceQuantity;
                TotExpandBalance += item.ExpendBalance;
                TotEndingBalancePiece += item.EndingBalancePiece;
                TotEndingBalance += item.EndingBalance;


                //result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Information, item.Buyer,

                //    item.BeginningBalanceQty, item.BeginningBalanceUom, item.ReceiptQty, item.ReceiptCorrectionQty, item.ReceiptUom,
                //    NumberFormat(item.ExpendQty),
                //    item.ExpandUom, item.EndingBalanceQty, item.EndingUom, item.From);


                result.Rows.Add(item.Nomor, item.Construction, "","", item.Grade, item.Piece, item.BeginingBalancePiece, item.BeginingBalance, item.ReceiptPieceQuantity, item.ReceiptBalance,  item.ExpendPieceQuantity,
                                item.ExpendBalance, item.EndingBalancePiece, item.EndingBalance
                                );

            }


            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");


            var col = (char)('A' + (result.Columns.Count - 1));
            //string tglawal = new DateTimeOffset(dateFrom.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //string tglakhir = new DateTimeOffset(dateTo.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("SALDO AKHIR GUDANG GREY");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("PERIODE : {0}", Bulan);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("BAGIAN : WEAVING 3 / AJL");
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells[$"A4:{col}4"].Value = string.Format("PER TGL. : {0}", Tanggal);
            sheet.Cells[$"A4:{col}4"].Merge = true;
            sheet.Cells[$"A4:{col}4"].Style.Font.Bold = true;
            sheet.Cells[$"A4:{col}4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A4:{col}4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells[$"A5:{col}5"].Value = string.Format("BARANG : GREY");
            sheet.Cells[$"A5:{col}5"].Merge = true;
            sheet.Cells[$"A5:{col}5"].Style.Font.Bold = true;
            sheet.Cells[$"A5:{col}5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[$"A5:{col}5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            sheet.Cells["A9"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells["G7"].Value = headers[6];
            sheet.Cells["G7:H7"].Merge = true;

            sheet.Cells["I7"].Value = headers[8];
            sheet.Cells["I7:J7"].Merge = true;

            sheet.Cells["K7"].Value = headers[10];
            sheet.Cells["K7:L7"].Merge = true;

            sheet.Cells["M7"].Value = headers[12];
            sheet.Cells["M7:N7"].Merge = true;





            foreach (var i in Enumerable.Range(0, 6))
            {
                col = (char)('A' + i);
                sheet.Cells[$"{col}7"].Value = headers[i];
                sheet.Cells[$"{col}7:{col}8"].Merge = true;
            }

            for (var i = 0; i < 8; i++)
            {
                col = (char)('G' + i);
                sheet.Cells[$"{col}8"].Value = subheaders[i];

            }

            //foreach (var i in Enumerable.Range(0, 1))
            //{
            //    col = (char)('P' + i);
            //    sheet.Cells[$"{col}5"].Value = headers[i + 15];
            //    sheet.Cells[$"{col}5:{col}6"].Merge = true;
            //}

            sheet.Cells["A7:N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A7:N8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A7:N8"].Style.Font.Bold = true;
            var widths = new int[] { 10, 30, 5, 5, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            //sheet.Column(5).Hidden = true;

            var a = Query.Count();
            var date = a + 2;
            var dateNow = DateTime.Now.Date;


            //sheet.Cells[$"A{5}:J{5 + a + 3}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            sheet.Cells[$"A{6}:N{7 + a + 2}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{7}:O{7 + a + 2}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            sheet.Cells[$"A{9 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{9 + a}:F{9 + a}"].Merge = true;
            sheet.Cells[$"A{9 + a}:N{9 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{9 + a}:F{9 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{9 + a}:F{9 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"G{9 + a}"].Value = TotBeginingBalancePiece;
            sheet.Cells[$"H{9 + a}"].Value = TotBeginingBalance;
            sheet.Cells[$"I{9 + a}"].Value = TotReceiptPieceQuantity;
            sheet.Cells[$"J{9 + a}"].Value = TotReceiptBalance;
            sheet.Cells[$"K{9 + a}"].Value = TotExpandPieceQuantity;
            sheet.Cells[$"L{9 + a}"].Value = TotExpandBalance;
            sheet.Cells[$"M{9 + a}"].Value = TotEndingBalancePiece;
            sheet.Cells[$"N{9 + a}"].Value = TotEndingBalance;
            




            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }


    }
}
