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
    public class InventoryMovementService : IInventoryMovementService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryMovement> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public InventoryMovementService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<InventoryMovement>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> Create(InventoryMovement model)
        {
            int Created = 0;

            var internalTransaction = DbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? DbContext.Database.CurrentTransaction : DbContext.Database.BeginTransaction();

            try
            {
                model.No = GenerateNo(model);
                model.FlagForCreate(IdentityService.Username, UserAgent);
                model.FlagForUpdate(IdentityService.Username, UserAgent);
                IInventorySummaryService summary = ServiceProvider.GetService<IInventorySummaryService>();

                this.DbSet.Add(model);
                Created = await DbContext.SaveChangesAsync();

                //var SumQty = this.dbSet.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId).Sum(a => a.Quantity);
                var SumQty = this.DbSet.OrderByDescending(a => a._CreatedUtc).FirstOrDefault(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId);

                var SumStock = this.DbSet.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId).Sum(a => a.StockPlanning);
                InventorySummary summaryModel = new InventorySummary
                {
                    ProductId = model.ProductId,
                    ProductCode = model.ProductCode,
                    ProductName = model.ProductName,
                    UomId = model.UomId,
                    UomUnit = model.UomUnit,
                    StockPlanning = SumStock,
                    Quantity = SumQty.After,
                    StorageId = model.StorageId,
                    StorageCode = model.StorageCode,
                    StorageName = model.StorageName
                };
                await summary.Create(summaryModel);
                if (internalTransaction)
                    transaction.Commit();

                return Created;
            }
            catch (Exception e)
            {
                if (internalTransaction)
                    transaction.Rollback();
                throw new Exception(e.Message);
            }
        }

        public MemoryStream GenerateExcel(string storageCode, string productCode, string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, type, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "UOM", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Before", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantiti", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "After", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Type", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0, 0, 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.storageName, item.referenceNo, item.referenceType, item.date.ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.productName, item.uomUnit, item.before,
                        item.quantity, item.after, item.type);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public Tuple<List<InventoryMovementViewModel>, int> GetReport(string storageCode, string productCode, string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, type, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<InventoryMovementViewModel> pageable = new Pageable<InventoryMovementViewModel>(Query, page - 1, size);
            List<InventoryMovementViewModel> Data = pageable.Data.ToList<InventoryMovementViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public InventoryMovement MapToModel(InventoryMovementViewModel viewModel)
        {
            var model = new InventoryMovement()
            {
                No = viewModel.no,
                Date = viewModel.date,
                ReferenceNo = viewModel.referenceNo,
                ReferenceType = viewModel.referenceType,
                ProductId = viewModel.productId,
                ProductCode = viewModel.productCode,
                ProductName = viewModel.productName,
                UomId = viewModel.uomId,
                UomUnit = viewModel.uomUnit,
                StorageId = viewModel.storageId,
                StorageCode = viewModel.storageCode,
                StorageName = viewModel.storageName,
                StockPlanning = viewModel.stockPlanning,
                Before = viewModel.before,
                Quantity = viewModel.quantity,
                After = viewModel.after,
                Remark = viewModel.remark,
                Type = viewModel.type
            };

            PropertyCopier<InventoryMovementViewModel, InventoryMovement>.Copy(viewModel, model);
            return model;
        }

        public InventoryMovementViewModel MapToViewModel(InventoryMovement model)
        {
            var viewModel = new InventoryMovementViewModel()
            {
                no = model.No,
                date = model.Date,
                referenceNo = model.ReferenceNo,
                referenceType = model.ReferenceType,
                productId = model.ProductId,
                productCode = model.ProductCode,
                productName = model.ProductName,
                uomId = model.UomId,
                uomUnit = model.UomUnit,
                storageId = model.StorageId,
                storageCode = model.StorageCode,
                storageName = model.StorageName,
                stockPlanning = model.StockPlanning,
                before = model.Before,
                quantity = model.Quantity,
                after = model.After,
                remark = model.Remark,
                type = model.Type,
                _LastModifiedUtc = model._LastModifiedUtc
            };
            PropertyCopier<InventoryMovement, InventoryMovementViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public ReadResponse<InventoryMovement> Read(int page, int size, string order, string keyword, string filter)
        {
            IQueryable<InventoryMovement> Query = this.DbSet;
            List<string> SelectedFields = new List<string>()
            {
                "Id", "no", "referenceNo", "referenceType", "date", "storageCode", "storageId", "storageName", "productCode", "productId", "productName", "quantity", "stockPlanning", "before", "after"
            };
            Query = Query
                .Select(s => new InventoryMovement
                {
                    Id = s.Id,
                    No = s.No,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Date = s.Date,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    ProductCode = s.ProductCode,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    StockPlanning = s.StockPlanning,
                    Before = s.Before,
                    After = s.After
                });

            List<string> searchAttributes = new List<string>()
            {
                "No", "ReferenceNo", "StorageName","ReferenceType"
            };

            Query = QueryHelper<InventoryMovement>.Search(Query, searchAttributes, keyword);
            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<InventoryMovement>.Order(Query, OrderDictionary);

            #endregion OrderBy

            #region Paging

            Pageable<InventoryMovement> pageable = new Pageable<InventoryMovement>(Query, page - 1, size);
            List<InventoryMovement> Data = pageable.Data.ToList<InventoryMovement>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            return new ReadResponse<InventoryMovement>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public InventoryMovement ReadModelById(int id)
        {
            var a = this.DbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .FirstOrDefault();
            return a;
        }

        string GenerateNo(InventoryMovement model)
        {
            do
            {
                model.No = CodeGenerator.GenerateCode();
            }
            while (this.DbSet.Any(d => d.No.Equals(model.No)));

            return model.No;
            //string Year = model.Date.ToString("yy");
            //string Month = model.Date.ToString("MM");


            //string no = $"MOV-{Year}-{Month}-{model.StorageCode}-";
            //int Padding = 7;

            //var lastNo = await this.dbSet.Where(w => w.No.StartsWith(no) && !w._IsDeleted).OrderByDescending(o => o.No).FirstOrDefaultAsync();

            //if (lastNo == null)
            //{
            //    return no + "1".PadLeft(Padding, '0');
            //}
            //else
            //{
            //    int lastNoNumber = Int32.Parse(lastNo.No.Replace(no, "")) + 1;
            //    return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            //}
        }

        public IQueryable<InventoryMovementViewModel> GetReportQuery(string storageCode, string productCode, string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in DbContext.InventoryMovements
                             //Conditions
                         where a._IsDeleted == false
                             && a.StorageCode == (string.IsNullOrWhiteSpace(storageCode) ? a.StorageCode : storageCode)
                             && a.ProductCode == (string.IsNullOrWhiteSpace(productCode) ? a.ProductCode : productCode)
                             && a.Type == (string.IsNullOrWhiteSpace(type) ? a.Type : type)
                             && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                         select new InventoryMovementViewModel
                         {
                             no = a.No,
                             date = a.Date,
                             referenceNo = a.ReferenceNo,
                             referenceType = a.ReferenceType,
                             productId = a.ProductId,
                             productCode = a.ProductCode,
                             productName = a.ProductName,
                             uomId = a.UomId,
                             uomUnit = a.UomUnit,
                             storageId = a.StorageId,
                             storageCode = a.StorageCode,
                             storageName = a.StorageName,
                             stockPlanning = a.StockPlanning,
                             before = a.Before,
                             quantity = a.Quantity,
                             after = a.After,
                             remark = a.Remark,
                             type = a.Type,
                             _LastModifiedUtc = a._LastModifiedUtc
                         });
            return Query;
        }

        public async Task<int> RefreshInventoryMovement()
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    //int result = 0;
                    List<InventoryMovement> dbMovement = await DbContext.InventoryMovements.ToListAsync();

                    foreach (var groupedItem in dbMovement.GroupBy(x => new { x.StorageId, x.ProductId, x.UomId }).ToList())
                    {
                        var orderedItem = groupedItem.OrderBy(x => x._CreatedUtc).ThenBy(x => x.Id).ToList();
                        //result += orderedItem.Count;
                        for (int i = 1; i < orderedItem.Count; i++)
                        {
                            var item = orderedItem[i];
                            var previousItem = orderedItem[i - 1];
                            item.Before = previousItem.After;
                            item.After = item.Before + item.Quantity;
                        }
                    }
                    DbContext.UpdateRange(dbMovement);
                    var result = await DbContext.SaveChangesAsync();
                    transaction.Commit();

                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
                finally
                {
                    DbContext.Dispose();
                }
            }

        }
    }
}
