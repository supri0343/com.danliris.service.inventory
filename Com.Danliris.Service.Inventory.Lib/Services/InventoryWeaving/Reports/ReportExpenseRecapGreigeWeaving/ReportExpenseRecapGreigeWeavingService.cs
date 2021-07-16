using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportExpenseRecapGreigeWeaving
{
    public class ReportExpenseRecapGreigeWeavingService : IReportExpenseRecapGreigeWeavingService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public ReportExpenseRecapGreigeWeavingService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public Tuple<List<ExpenseRecapReportViewModel>, int> GetReportExpenseRecap(string bonType, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            //var query = GetReport(bonType, dateFrom, dateTo, offset);
            List<ExpenseRecapReportViewModel> query = GetReport(bonType, dateFrom, dateTo, offset);


            Pageable<ExpenseRecapReportViewModel> pageable = new Pageable<ExpenseRecapReportViewModel>(query, page - 1, size);
            List<ExpenseRecapReportViewModel> Data = pageable.Data.ToList<ExpenseRecapReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public List<ExpenseRecapReportViewModel> GetReport(string bonType, DateTime? dateFrom, DateTime? dateTo, int offset)
        {

            //IQueryable<InventoryWeavingMovement> Query = this.DbSetMovement;

            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var result = (from a in DbSetDoc
                          join b in DbSetMovement on a.Id equals b.InventoryWeavingDocumentId
                          where a._IsDeleted == false
                             && b._IsDeleted == false
                             && a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
                               && a.Date.AddHours(offset).Date >= DateFrom.Date
                              && a.Date.AddHours(offset).Date <= DateTo.Date
                          orderby a.Date, a._CreatedUtc ascending
                          select new ExpenseRecapReportViewModel
                          {
                              Construction = b.Construction,
                              Grade = b.Grade,
                              Type = b.Type,
                              MaterialName = b.MaterialName,
                              WovenType = b.WovenType,
                              Yarn1 = b.Yarn1,
                              Yarn2 = b.Yarn2,
                              YarnOrigin1 = b.YarnOrigin1,
                              YarnOrigin2 = b.YarnOrigin2,
                              YarnType1 = b.YarnType1,
                              YarnType2 = b.YarnType2,
                              Qty = b.Quantity,
                              QtyPiece = b.QuantityPiece


                          });

            var data1 = result.GroupBy(s => new
            {
                s.Grade,
                s.Type,
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new ExpenseRecapReportViewModel()
            {
                Construction = s.FirstOrDefault().Construction,
                Grade = s.FirstOrDefault().Grade,
                Type = s.Key.Type,
                Qty = s.Sum(d => d.Qty),
                QtyPiece = s.Sum(d => d.QtyPiece),
                MaterialName = s.Key.MaterialName,
                WovenType = s.Key.WovenType,
                Width = s.Key.Width,
                Yarn1 = s.Key.Yarn1,
                Yarn2 = s.Key.Yarn2,
                YarnOrigin1 = s.Key.YarnOrigin1,
                YarnOrigin2 = s.Key.YarnOrigin2,
                YarnType1 = s.Key.YarnType1,
                YarnType2 = s.Key.YarnType2



            });

             var Data = data1.GroupBy(s => new
            {

                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
            }).Select(s => new ExpenseRecapReportViewModel()
            {
                Construction = s.FirstOrDefault().Construction,
                Grade = s.FirstOrDefault().Grade,
                Qty = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").Qty - s.FirstOrDefault(d => d.Type == "OUT").Qty :
                                          s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").Qty : 0,
                QtyPiece = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece - s.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                                             s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece : 0

            }).Where(x => x.Qty > 0 && x.QtyPiece > 0).ToList();

            List<ExpenseRecapReportViewModel> reportViewModels = new List<ExpenseRecapReportViewModel>();
            //int index = 1;

            foreach (var i in Data)
            {
                reportViewModels.Add(new ExpenseRecapReportViewModel
                {
                    Construction = i.Construction,
                    Grade = i.Grade,
                    Qty = i.Qty,
                    QtyPiece = i.QtyPiece

                });
            }

            var total = new ExpenseRecapReportViewModel
            {
                Construction = "Total:",
                Grade = "",
                QtyPiece = Math.Round(Data.Sum(x => x.QtyPiece), 2),
                Qty = Math.Round(Data.Sum(x => x.Qty), 2)


            };

            reportViewModels.Add(total);


            return reportViewModels;

            //return Data.ToList();

        }

        public MemoryStream GenerateExcelExpenseRecap(string bonType, DateTime? dateFrom, DateTime? dateTo, int offSet)
        {
            var Query = GetReport(bonType, dateFrom, dateTo, offSet);

            DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn() { ColumnName = "No. SC", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Grade", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kg", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Ball", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jml. Piece", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jml. Meter", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Ket", DataType = typeof(string) });

            if (Query.Count() == 0)
            {
                dt.Rows.Add( "", "", "", "", 0, 0, "");
            }
            else
            {
                foreach (var model in Query)
                {
                    // string date = model.Date == null ? "-" : model.Date.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    dt.Rows.Add(model.Construction, model.Grade, "", "", model.QtyPiece, model.Qty, "");
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Inventory Weaving") }, true);
        }
    }
}
