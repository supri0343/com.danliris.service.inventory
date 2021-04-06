using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace Com.Danliris.Service.Inventory.Lib.Services.Inventory
{
    public class InventoryDystuffService : IInventoryDystuffService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryMovement> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public InventoryDystuffService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<InventoryMovement>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }


        public IQueryable<InventoryDystuffViewModel> GetReportQuery(string storageCode, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in DbContext.InventoryMovements
                             //Conditions
                         where a._IsDeleted == false
                             && a.StorageCode == (string.IsNullOrWhiteSpace(storageCode) ? a.StorageCode : storageCode)
                             && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date

                         select new InventoryDystuffViewModel
                         {


                             ProductCode = a.ProductCode,
                             ProductName = a.ProductName,
                             UomUnit = a.UomUnit,
                             BeginningQty = a.Before,
                             ReceiptQty = Math.Abs(a.Type == "IN" ? a.After - a.Before : 0),
                             ExpandQty = Math.Abs(a.Type == "OUT" ? a.After - a.Before : 0),
                             EndingQty = a.After,
                             Date = a.Date,
                             Type = a.Type
                         });
            return Query;
        }

        public Tuple<List<InventoryDystuffViewModel>, int> GetReport(string storageCode, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(storageCode, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.Date);
            }


            Pageable<InventoryDystuffViewModel> pageable = new Pageable<InventoryDystuffViewModel>(Query, page - 1, size);
            List<InventoryDystuffViewModel> Data = pageable.Data.ToList<InventoryDystuffViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }


        public MemoryStream GenerateExcel(string storageCode, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(storageCode, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.Date);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Masuk", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keluar", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo AKhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                //   result.Rows.Add("", "", "", "", "", "", "", 0, 0, 0, ""); // to allow column name to be generated properly for empty data as template
                result.Rows.Add("", "", "", "", "", 0, 0, 0, 0, "");
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string date = item.Date == null ? "-" : item.Date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, date, item.ProductCode, item.ProductName, item.UomUnit, item.BeginningQty,
                        item.ReceiptQty, item.ExpandQty, item.EndingQty, item.Type);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }


    }
}
