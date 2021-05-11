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
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.Inventory
{
    public class InventoryDocumentService : IInventoryDocumentService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryDocument> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public InventoryDocumentService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<InventoryDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> Create(InventoryDocument model)
        {
            int Created = 0;
            var internalTransaction = DbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? DbContext.Database.CurrentTransaction : DbContext.Database.BeginTransaction();

            try
            {
                IInventoryMovementService movement = ServiceProvider.GetService<IInventoryMovementService>();

                model.No = GenerateNo(model);
                model.FlagForCreate(IdentityService.Username, UserAgent);
                model.FlagForUpdate(IdentityService.Username, UserAgent);

                foreach (var item in model.Items)
                {
                    item.FlagForCreate(IdentityService.Username, UserAgent);
                    item.FlagForUpdate(IdentityService.Username, UserAgent);
                }

                DbSet.Add(model);
                Created = await DbContext.SaveChangesAsync();
                foreach (var item in model.Items)
                {
                    var qty = item.Quantity;
                    if (model.Type == "OUT")
                    {
                        qty = item.Quantity * -1;
                    }
                    var SumQty = DbContext.InventoryMovements.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == item.ProductId && a.UomId == item.UomId).Sum(a => a.Quantity);

                    InventoryMovement movementModel = new InventoryMovement
                    {
                        ProductCode = item.ProductCode,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        StorageCode = model.StorageCode,
                        StorageId = model.StorageId,
                        StorageName = model.StorageName,
                        Before = SumQty,
                        Quantity = qty,
                        After = SumQty + qty,
                        ReferenceNo = model.ReferenceNo,
                        ReferenceType = model.ReferenceType,
                        Type = model.Type,
                        Date = model.Date,
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        Remark = item.ProductRemark
                    };
                    await movement.Create(movementModel);
                }
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

        public InventoryDocument MapToModel(InventoryDocumentViewModel viewModel)
        {
            InventoryDocument model = new InventoryDocument
            {
                ReferenceNo = viewModel.referenceNo,
                ReferenceType = viewModel.referenceType,
                Remark = viewModel.remark,
                StorageCode = viewModel.storageCode,
                StorageId = Convert.ToInt32(viewModel.storageId),
                StorageName = viewModel.storageName,
                Date = viewModel.date,
                Type = viewModel.type,
                Items = viewModel.items.Select(item => new InventoryDocumentItem()
                {
                    Id = item.Id,
                    Active = item.Active,
                    _CreatedAgent = item._CreatedAgent,
                    _CreatedBy = item._CreatedBy,
                    _CreatedUtc = item._CreatedUtc,
                    _IsDeleted = item._IsDeleted,
                    _LastModifiedAgent = item._LastModifiedAgent,
                    _LastModifiedBy = item._LastModifiedBy,
                    _LastModifiedUtc = item._LastModifiedUtc,
                    ProductCode = item.productCode,
                    ProductId = item.productId,
                    ProductName = item.productName,
                    ProductRemark = item.remark,
                    Quantity = item.quantity,
                    StockPlanning = item.stockPlanning,
                    UomId = item.uomId,
                    UomUnit = item.uom,
                }).ToList()
            };
            PropertyCopier<InventoryDocumentViewModel, InventoryDocument>.Copy(viewModel, model);
            return model;
        }

        public InventoryDocumentViewModel MapToViewModel(InventoryDocument model)
        {
            InventoryDocumentViewModel viewModel = new InventoryDocumentViewModel
            {
                no = model.No,
                referenceNo = model.ReferenceNo,
                referenceType = model.ReferenceType,
                remark = model.Remark,
                storageCode = model.StorageCode,
                storageId = model.StorageId,
                storageName = model.StorageName,
                date = model.Date,
                type = model.Type,
                items = model.Items.Select(item => new InventoryDocumentItemViewModel()
                {
                    Id = item.Id,
                    Active = item.Active,
                    _CreatedAgent = item._CreatedAgent,
                    _CreatedBy = item._CreatedBy,
                    _CreatedUtc = item._CreatedUtc,
                    _IsDeleted = item._IsDeleted,
                    _LastModifiedAgent = item._LastModifiedAgent,
                    _LastModifiedBy = item._LastModifiedBy,
                    _LastModifiedUtc = item._LastModifiedUtc,
                    productCode = item.ProductCode,
                    productId = item.ProductId,
                    productName = item.ProductName,
                    remark = item.ProductRemark,
                    quantity = item.Quantity,
                    stockPlanning = item.StockPlanning,
                    uomId = item.UomId,
                    uom = item.UomUnit,
                }).ToList()
            };

            PropertyCopier<InventoryDocument, InventoryDocumentViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public ReadResponse<InventoryDocument> Read(int page, int size, string order, string keyword, string filter)
        {
            IQueryable<InventoryDocument> Query = this.DbSet;

            List<string> SearchAttributes = new List<string>()
                {
                    "No", "ReferenceNo", "StorageName","ReferenceType","Type"
                };
            Query = QueryHelper<InventoryDocument>.Search(Query, SearchAttributes, keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "no", "referenceNo", "referenceType", "date", "storageCode", "storageId", "storageName", "type", "_LastModifiedUtc", "items"
            };

            Query = Query
                .Select(s => new InventoryDocument
                {
                    Id = s.Id,
                    No = s.No,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Date = s.Date,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    Type = s.Type,
                    _LastModifiedUtc = s._LastModifiedUtc,
                    Items = s.Items.Select(a => new InventoryDocumentItem
                    {
                        Quantity = a.Quantity,
                        ProductCode = a.ProductCode,
                        ProductId = a.ProductId,
                        ProductName = a.ProductName,
                        UomId = a.UomId,
                        UomUnit = a.UomUnit,
                    }).ToList()
                });


            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<InventoryDocument>.Order(Query, OrderDictionary);
            #endregion OrderBy

            #region Paging

            Pageable<InventoryDocument> pageable = new Pageable<InventoryDocument>(Query, page - 1, size);
            List<InventoryDocument> Data = pageable.Data.ToList<InventoryDocument>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            return new ReadResponse<InventoryDocument>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public InventoryDocument ReadModelById(int id)
        {
            var a = this.DbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                 .Include(p => p.Items)
                 .FirstOrDefault();
            return a;
        }

        public async Task<int> CreateMulti(List<InventoryDocument> models)
        {
            try
            {
                int Created = 0;

                foreach (var item in models)
                {
                    Created += await Create(item);
                }
                return Created;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string GenerateNo(InventoryDocument model)
        {

            do
            {
                model.No = CodeGenerator.GenerateCode();
            }
            while (this.DbSet.Any(d => d.No.Equals(model.No)));

            return model.No;
            //string Year = model.Date.ToString("yy");
            //string Month = model.Date.ToString("MM");



            //string no = $"DOC-{Year}-{Month}-{model.StorageCode}-";
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

    }
}
