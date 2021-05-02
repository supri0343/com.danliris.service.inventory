using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService
{
    public class NewStockTransferNoteService : IStockTransferNoteService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<StockTransferNote> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public NewStockTransferNoteService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<StockTransferNote>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(StockTransferNote model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.Code = CodeGenerator.GenerateCode();
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);
                    foreach (var item in model.StockTransferNoteItems)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }

                    DbSet.Add(model);

                    Created = await DbContext.SaveChangesAsync();
                    await CreateInventoryDocument(model, "OUT", "CREATE");

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return Created;
        }

        public async Task<int> DeleteAsync(int id)
        {
            int Count = 0;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    StockTransferNote stockTransferNote = await ReadByIdAsync(id);
                    stockTransferNote.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in stockTransferNote.StockTransferNoteItems)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }
                    Count = await DbContext.SaveChangesAsync();


                    //StockTransferNoteService stockTransferNoteService = ServiceProvider.GetService<StockTransferNoteService>();
                    //StockTransferNote_ItemService stockTransferNoteItemService = ServiceProvider.GetService<StockTransferNote_ItemService>();
                    //stockTransferNoteItemService.Username = this.Username;

                    //HashSet<int> StockTransferNoteItems = new HashSet<int>(this.DbContext.StockTransferNoteItems.Where(p => p.StockTransferNote.Equals(Id)).Select(p => p.Id));

                    //foreach (int item in StockTransferNoteItems)
                    //{
                    //    await stockTransferNoteItemService.DeleteAsync(item);
                    //}

                    await CreateInventoryDocument(stockTransferNote, "IN", "DELETE-SOURCE");

                    Transaction.Commit();
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    throw e;
                }
            }

            return Count;
        }

        public StockTransferNote MapToModel(StockTransferNoteViewModel viewModel)
        {
            StockTransferNote model = new StockTransferNote();
            PropertyCopier<StockTransferNoteViewModel, StockTransferNote>.Copy(viewModel, model);

            model.SourceStorageId = viewModel.SourceStorage._id;
            model.SourceStorageCode = viewModel.SourceStorage.code;
            model.SourceStorageName = viewModel.SourceStorage.name;
            model.TargetStorageId = viewModel.TargetStorage._id;
            model.TargetStorageCode = viewModel.TargetStorage.code;
            model.TargetStorageName = viewModel.TargetStorage.name;

            model.StockTransferNoteItems = new List<StockTransferNote_Item>();
            foreach (StockTransferNote_ItemViewModel stn in viewModel.StockTransferNoteItems)
            {
                StockTransferNote_Item stockTransferNoteItem = new StockTransferNote_Item();
                PropertyCopier<StockTransferNote_ItemViewModel, StockTransferNote_Item>.Copy(stn, stockTransferNoteItem);

                stockTransferNoteItem.ProductId = stn.Summary.productId.ToString();
                stockTransferNoteItem.ProductCode = stn.Summary.productCode;
                stockTransferNoteItem.ProductName = stn.Summary.productName;
                stockTransferNoteItem.UomId = stn.Summary.uomId.ToString();
                stockTransferNoteItem.UomUnit = stn.Summary.uom;
                stockTransferNoteItem.StockQuantity = stn.Summary.quantity;
                stockTransferNoteItem.TransferedQuantity = stn.TransferedQuantity != null ? (double)stn.TransferedQuantity : 0;

                model.StockTransferNoteItems.Add(stockTransferNoteItem);
            }

            return model;
        }

        public StockTransferNoteViewModel MapToViewModel(StockTransferNote model)
        {
            StockTransferNoteViewModel viewModel = new StockTransferNoteViewModel();
            PropertyCopier<StockTransferNote, StockTransferNoteViewModel>.Copy(model, viewModel);

            StorageViewModel SourceStorage = new StorageViewModel()
            {
                _id = model.SourceStorageId,
                code = model.SourceStorageCode,
                name = model.SourceStorageName
            };

            StorageViewModel TargetStorage = new StorageViewModel()
            {
                _id = model.TargetStorageId,
                code = model.TargetStorageCode,
                name = model.TargetStorageName
            };

            viewModel.SourceStorage = SourceStorage;
            viewModel.TargetStorage = TargetStorage;

            viewModel.StockTransferNoteItems = new List<StockTransferNote_ItemViewModel>();
            if (model.StockTransferNoteItems != null)
            {
                foreach (StockTransferNote_Item stn in model.StockTransferNoteItems)
                {
                    StockTransferNote_ItemViewModel stockTransferNoteItemViewModel = new StockTransferNote_ItemViewModel();
                    PropertyCopier<StockTransferNote_Item, StockTransferNote_ItemViewModel>.Copy(stn, stockTransferNoteItemViewModel);
                    InventorySummaryViewModel Summary = new InventorySummaryViewModel()
                    {
                        productId = int.TryParse(stn.ProductId, out int prdId) ? prdId : 0,
                        productCode = stn.ProductCode,
                        productName = stn.ProductName,
                        quantity = stn.StockQuantity,
                        uomId = int.TryParse(stn.UomId, out int unitId) ? unitId : 0,
                        uom = stn.UomUnit
                    };

                    stockTransferNoteItemViewModel.Summary = Summary;

                    viewModel.StockTransferNoteItems.Add(stockTransferNoteItemViewModel);
                }
            }

            return viewModel;
        }

        public ReadResponse<StockTransferNote> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<StockTransferNote> Query = this.DbContext.StockTransferNotes;

            List<string> SearchAttributes = new List<string>()
            {
                "Code"
            };

            Query = QueryHelper<StockTransferNote>.Search(Query, SearchAttributes, keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "_CreatedUtc", "ReferenceNo", "ReferenceType", "SourceStorage", "TargetStorage", "IsApproved", "_LastModifiedUtc"
            };

            Query = Query
                .Select(stn => new StockTransferNote
                {
                    Id = stn.Id,
                    Code = stn.Code,
                    _CreatedUtc = stn._CreatedUtc,
                    ReferenceNo = stn.ReferenceNo,
                    ReferenceType = stn.ReferenceType,
                    SourceStorageName = stn.SourceStorageName,
                    TargetStorageName = stn.TargetStorageName,
                    IsApproved = stn.IsApproved,
                    _LastModifiedUtc = stn._LastModifiedUtc
                });

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<StockTransferNote>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<StockTransferNote>.Order(Query, OrderDictionary);

            Pageable<StockTransferNote> pageable = new Pageable<StockTransferNote>(Query, page - 1, size);
            List<StockTransferNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<StockTransferNote>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<StockTransferNote> ReadByIdAsync(int id)
        {
            return await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.StockTransferNoteItems)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, StockTransferNote model)
        {
            int updated = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (id != model.Id)
                        throw new Exception("data not found");

                    var dbModel = await ReadByIdAsync(id);

                    dbModel.IsApproved = model.IsApproved;
                    dbModel.ReferenceNo = model.ReferenceNo;
                    dbModel.ReferenceType = model.ReferenceType;
                    dbModel.SourceStorageCode = model.SourceStorageCode;
                    dbModel.SourceStorageId = model.SourceStorageId;
                    dbModel.SourceStorageName = model.SourceStorageName;
                    dbModel.TargetStorageCode = model.TargetStorageCode;
                    dbModel.TargetStorageId = model.TargetStorageId;
                    dbModel.TargetStorageName = model.TargetStorageName;

                    dbModel.FlagForUpdate(IdentityService.Username, UserAgent);
                    //DbSet.Update(dbModel);

                    var deletedItems = dbModel.StockTransferNoteItems.Where(x => !model.StockTransferNoteItems.Any(y => x.Id == y.Id));
                    var updatedItems = dbModel.StockTransferNoteItems.Where(x => model.StockTransferNoteItems.Any(y => x.Id == y.Id));
                    var addedItems = model.StockTransferNoteItems.Where(x => !dbModel.StockTransferNoteItems.Any(y => y.Id == x.Id));

                    foreach (var item in deletedItems)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);

                    }

                    foreach (var item in updatedItems)
                    {
                        var selectedItem = model.StockTransferNoteItems.FirstOrDefault(x => x.Id == item.Id);

                        item.ProductCode = selectedItem.ProductCode;
                        item.ProductId = selectedItem.ProductId;
                        item.ProductName = selectedItem.ProductName;
                        item.StockQuantity = selectedItem.StockQuantity;
                        item.TransferedQuantity = selectedItem.TransferedQuantity;
                        item.UomId = selectedItem.UomId;
                        item.UomUnit = selectedItem.UomUnit;
                        item.FlagForUpdate(IdentityService.Username, UserAgent);

                    }

                    foreach (var item in addedItems)
                    {
                        item.StockTransferNoteId = id;
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);

                    }
                    updated = await DbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return updated;
        }

        public Tuple<List<StockTransferNote>, int, Dictionary<string, string>, List<string>> ReadModelByNotUser(int Page, int Size, string Order, List<string> Select, string Keyword, string Filter)
        {
            IQueryable<StockTransferNote> Query = this.DbContext.StockTransferNotes;

            List<string> SearchAttributes = new List<string>()
            {
                "Code"
            };

            Query = QueryHelper<StockTransferNote>.Search(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "_CreatedUtc", "ReferenceNo", "ReferenceType", "SourceStorage", "TargetStorage", "IsApproved", "_LastModifiedUtc", "_CreatedBy"
            };

            Query = Query
                .Select(stn => new StockTransferNote
                {
                    Id = stn.Id,
                    Code = stn.Code,
                    _CreatedUtc = stn._CreatedUtc,
                    ReferenceNo = stn.ReferenceNo,
                    ReferenceType = stn.ReferenceType,
                    SourceStorageName = stn.SourceStorageName,
                    TargetStorageName = stn.TargetStorageName,
                    IsApproved = stn.IsApproved,
                    _LastModifiedUtc = stn._LastModifiedUtc,
                    _CreatedBy = stn._CreatedBy
                }).Where(w => !string.Equals(w._CreatedBy, IdentityService.Username));

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = QueryHelper<StockTransferNote>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<StockTransferNote>.Order(Query, OrderDictionary);

            Pageable<StockTransferNote> pageable = new Pageable<StockTransferNote>(Query, Page - 1, Size);
            List<StockTransferNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<int> CreateInventoryDocument(StockTransferNote Model, string Type, string Context)
        {
            StockTransferNoteViewModel ViewModel = MapToViewModel(Model);

            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));

            /* Create Inventory Document */
            List<InventoryDocumentItem> inventoryDocumentItems = new List<InventoryDocumentItem>();

            foreach (StockTransferNote_ItemViewModel stni in ViewModel.StockTransferNoteItems)
            {
                InventoryDocumentItem inventoryDocumentItem = new InventoryDocumentItem
                {
                    ProductId = stni.Summary.productId,
                    ProductCode = stni.Summary.productCode,
                    ProductName = stni.Summary.productName,
                    Quantity = stni.TransferedQuantity != null ? (double)stni.TransferedQuantity : 0,
                    UomId = stni.Summary.uomId,
                    UomUnit = stni.Summary.uom
                };

                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            InventoryDocument inventoryDocument = new InventoryDocument
            {
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Model.ReferenceNo,
                ReferenceType = Model.ReferenceType,
                Type = Type,
                StorageId = string.Equals(Context.ToUpper(), "CREATE") || string.Equals(Context.ToUpper(), "DELETE-SOURCE") ? int.Parse(Model.SourceStorageId) : int.Parse(Model.TargetStorageId),
                StorageCode = string.Equals(Context.ToUpper(), "CREATE") || string.Equals(Context.ToUpper(), "DELETE-SOURCE") ? Model.SourceStorageCode : Model.TargetStorageCode,
                StorageName = string.Equals(Context.ToUpper(), "CREATE") || string.Equals(Context.ToUpper(), "DELETE-SOURCE") ? Model.SourceStorageName : Model.TargetStorageName,
                Items = inventoryDocumentItems
            };

            var inventoryDocumentFacade = ServiceProvider.GetService<IInventoryDocumentService>();
            return await inventoryDocumentFacade.Create(inventoryDocument);
        }

        public async Task<bool> UpdateIsApprove(int Id)
        {
            bool IsSuccessful = false;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var stockTransferNote = await this.ReadByIdAsync(Id);
                    stockTransferNote.IsApproved = true;
                    stockTransferNote.FlagForUpdate(IdentityService.Username, UserAgent);
                    this.DbContext.SaveChanges();
                    await CreateInventoryDocument(stockTransferNote, "IN", "APPROVE");

                    IsSuccessful = true;
                    Transaction.Commit();
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    throw e;
                }
            }

            return IsSuccessful;
        }
    }
}
