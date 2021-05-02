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
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.Inventory
{
    public class InventorySummaryService : IInventorySummaryService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<InventorySummary> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public InventorySummaryService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<InventorySummary>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> Create(InventorySummary model)
        {
            int Created = 0;
            var internalTransaction = DbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? DbContext.Database.CurrentTransaction : DbContext.Database.BeginTransaction();

            try
            {
                var exist = this.DbSet.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId).FirstOrDefault();

                if (exist == null)
                {
                    model.No = GenerateNo(model);
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    this.DbSet.Add(model);
                }
                else
                {
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    exist.Quantity = model.Quantity;
                    exist.StockPlanning = model.StockPlanning;
                    this.DbSet.Update(exist);
                }
                Created = await DbContext.SaveChangesAsync();

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

        public MemoryStream GenerateExcel(string storageCode, string productCode, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, offset);
            Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantiti", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "UOM", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    result.Rows.Add(index, item.storageName, item.productName, item.quantity, item.uom);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public List<InventorySummary> GetByStorageAndMTR(string storageName)
        {
            IQueryable<InventorySummary> Query = this.DbSet;

            Query = Query
                .Where(s => s.StorageName.Equals(storageName) && s.UomUnit.Equals("MTR"));

            return Query.ToList();
        }

        public List<InventorySummaryViewModel> GetInventorySummaries(string productIds = "{}")
        {
            Dictionary<string, object> productIdDictionaries = JsonConvert.DeserializeObject<Dictionary<string, object>>(productIds);
            if (productIdDictionaries.Count == 0)
                return new List<InventorySummaryViewModel>();
            var productIdString = productIdDictionaries.Values.FirstOrDefault();
            var productIdList = JsonConvert.DeserializeObject<List<int>>(productIdString.ToString());

            IQueryable<InventorySummary> rootQuery = DbContext.InventorySummaries.Where(x => !x._IsDeleted);
            List<InventorySummary> documentItems = new List<InventorySummary>();
            foreach (var id in productIdList)
            {
                var result = rootQuery.Where(x => x.ProductId == id);
                documentItems.AddRange(result.ToList());
            }
            return documentItems.Select(x => new InventorySummaryViewModel()
            {
                productCode = x.ProductCode,
                productId = x.ProductId,
                productName = x.ProductName,
                quantity = x.Quantity,
                storageCode = x.StorageCode,
                Active = x.Active,
                Id = x.Id,
                stockPlanning = x.StockPlanning.ToString(),
                storageId = x.StorageId,
                storageName = x.StorageName,
                code = x.No,
                uomId = x.UomId,
                uom = x.UomUnit,
            }).ToList();
        }

        public Tuple<List<InventorySummaryViewModel>, int> GetReport(string storageCode, string productCode, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, offset);

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


            Pageable<InventorySummaryViewModel> pageable = new Pageable<InventorySummaryViewModel>(Query, page - 1, size);
            List<InventorySummaryViewModel> Data = pageable.Data.ToList<InventorySummaryViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public InventorySummary GetSummaryByParams(int storageId, int productId, int uomId)
        {
            return DbSet.FirstOrDefault(f => f.StorageId.Equals(storageId) && f.ProductId.Equals(productId) && f.UomId.Equals(uomId));
        }

        public InventorySummary MapToModel(InventorySummaryViewModel viewModel)
        {
            var model = new InventorySummary()
            {
                No = viewModel.code,
                ProductId = viewModel.productId,
                ProductCode = viewModel.productCode,
                ProductName = viewModel.productName,
                UomId = viewModel.uomId,
                UomUnit = viewModel.uom,
                StorageId = viewModel.storageId,
                StorageCode = viewModel.storageCode,
                StorageName = viewModel.storageName,
                StockPlanning = double.Parse(viewModel.stockPlanning),
                Quantity = viewModel.quantity
            };

            PropertyCopier<InventorySummaryViewModel, InventorySummary>.Copy(viewModel, model);
            return model;
        }

        public InventorySummaryViewModel MapToViewModel(InventorySummary model)
        {
            var viewModel = new InventorySummaryViewModel()
            {
                code = model.No,
                productId = model.ProductId,
                productCode = model.ProductCode,
                productName = model.ProductName,
                uomId = model.UomId,
                uom = model.UomUnit,
                storageId = model.StorageId,
                storageCode = model.StorageCode,
                storageName = model.StorageName,
                stockPlanning = model.StockPlanning.ToString(),
                quantity = model.Quantity
                
            };

            PropertyCopier<InventorySummary, InventorySummaryViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public ReadResponse<InventorySummary> Read(int page, int size, string order, string keyword, string filter)
        {
            IQueryable<InventorySummary> Query = this.DbSet;

            Query = Query
                .Select(s => new InventorySummary
                {
                    Id = s.Id,
                    No = s.No,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    ProductCode = s.ProductCode,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    StockPlanning = s.StockPlanning
                });

            List<string> searchAttributes = new List<string>()
            {
                "No", "ReferenceNo", "StorageName","ReferenceType"
            };

            List<string> SelectedFields = new List<string>()
            {
                "Id", "no", "storageCode", "storageId", "storageName", "productCode", "productId", "productName", "quantity", "stockPlanning"
            };

            Query = QueryHelper<InventorySummary>.Search(Query, searchAttributes, keyword);
            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<InventorySummary>.Order(Query, OrderDictionary);


            #endregion OrderBy

            #region Paging

            Pageable<InventorySummary> pageable = new Pageable<InventorySummary>(Query, page - 1, size);
            List<InventorySummary> Data = pageable.Data.ToList<InventorySummary>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            return new ReadResponse<InventorySummary>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public InventorySummary ReadModelById(int id)
        {
            var a = this.DbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .FirstOrDefault();
            return a;
        }

        string GenerateNo(InventorySummary model)
        {
            do
            {
                model.No = CodeGenerator.GenerateCode();
            }
            while (this.DbSet.Any(d => d.No.Equals(model.No)));

            return model.No;
            //string Year = DateTimeOffset.Now.ToString("yy");
            //string Month = DateTimeOffset.Now.ToString("MM");


            //string no = $"SUM-{Year}-{Month}-{model.StorageCode}-";
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

        public IQueryable<InventorySummaryViewModel> GetReportQuery(string storageCode, string productCode, int offset)
        {
            var Query = (from a in DbContext.InventorySummaries
                             //Conditions
                         where a._IsDeleted == false
                             && a.StorageCode == (string.IsNullOrWhiteSpace(storageCode) ? a.StorageCode : storageCode)
                             && a.ProductCode == (string.IsNullOrWhiteSpace(productCode) ? a.ProductCode : productCode)
                         select new InventorySummaryViewModel
                         {
                             code = a.No,
                             productId = a.ProductId,
                             productCode = a.ProductCode,
                             productName = a.ProductName,
                             uomId = a.UomId,
                             uom = a.UomUnit,
                             storageId = a.StorageId,
                             storageCode = a.StorageCode,
                             storageName = a.StorageName,
                             quantity = a.Quantity,

                             _LastModifiedUtc = a._LastModifiedUtc
                         });
            return Query;
        }
    }
}
