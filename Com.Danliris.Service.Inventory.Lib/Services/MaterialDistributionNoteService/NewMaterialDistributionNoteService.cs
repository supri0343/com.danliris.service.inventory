using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService
{
    public class NewMaterialDistributionNoteService : IMaterialDistributionService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<MaterialDistributionNote> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public NewMaterialDistributionNoteService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<MaterialDistributionNote>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(MaterialDistributionNote model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    model = await this.CustomCodeGenerator(model);
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);
                    foreach (var item in model.MaterialDistributionNoteItems)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        foreach (var detail in item.MaterialDistributionNoteDetails)
                        {
                            detail.FlagForCreate(IdentityService.Username, UserAgent);
                            detail.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                    }

                    DbSet.Add(model);

                    Created = await DbContext.SaveChangesAsync();

                    IMaterialRequestNoteService materialsRequestNoteService = ServiceProvider.GetService<IMaterialRequestNoteService>();

                    List<ViewModels.InventoryViewModel.InventorySummaryViewModel> data = new List<ViewModels.InventoryViewModel.InventorySummaryViewModel>();


                    foreach (MaterialDistributionNoteItem materialDistributionNoteItem in model.MaterialDistributionNoteItems)
                    {
                        MaterialsRequestNote materialsRequestNote = await materialsRequestNoteService.ReadByIdAsync(materialDistributionNoteItem.MaterialRequestNoteId);
                        materialsRequestNote.IsDistributed = true;

                        if (model.Type.ToUpper().Equals("PRODUKSI"))
                        {
                            foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
                            {
                                materialsRequestNote.MaterialsRequestNote_Items.Where(w => w.ProductionOrderId.Equals(materialDistributionNoteDetail.ProductionOrderId)).Select(s => { s.DistributedLength += materialDistributionNoteDetail.ReceivedLength; return s; }).ToList();
                            }
                            materialsRequestNoteService.UpdateDistributedQuantity(materialsRequestNote.Id, materialsRequestNote);

                        }
                        await materialsRequestNoteService.UpdateAsync(materialsRequestNote.Id, materialsRequestNote);

                    }

                    DbContext.SaveChanges();

                    await CreateInventoryDocument(model, "OUT");


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
                    MaterialDistributionNote materialDistributionNote = await ReadByIdAsync(id);
                    materialDistributionNote.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in materialDistributionNote.MaterialDistributionNoteItems)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);

                        foreach (var detail in item.MaterialDistributionNoteDetails)
                        {
                            detail.FlagForDelete(IdentityService.Username, UserAgent);
                        }
                    }

                    Count = await DbContext.SaveChangesAsync();
                    //MaterialDistributionNoteItemService materialDistributionNoteItemService = ServiceProvider.GetService<MaterialDistributionNoteItemService>();
                    //MaterialDistributionNoteDetailService materialDistributionNoteDetailService = ServiceProvider.GetService<MaterialDistributionNoteDetailService>();
                    //materialDistributionNoteItemService.Username = this.Username;
                    //materialDistributionNoteDetailService.Username = this.Username;

                    //HashSet<int> MaterialDistributionNoteItems = new HashSet<int>(this.DbContext.MaterialDistributionNoteItems.Where(p => p.MaterialDistributionNoteId.Equals(Id)).Select(p => p.Id));

                    //List<InventorySummaryViewModel> data = new List<InventorySummaryViewModel>();

                    //foreach (int item in MaterialDistributionNoteItems)
                    //{
                    //    HashSet<int> MaterialDistributionNoteDetails = new HashSet<int>(this.DbContext.MaterialDistributionNoteDetails.Where(p => p.MaterialDistributionNoteItemId.Equals(item)).Select(p => p.Id));

                    //    foreach (int detail in MaterialDistributionNoteDetails)
                    //    {
                    //        await materialDistributionNoteDetailService.DeleteAsync(detail);
                    //    }

                    //    await materialDistributionNoteItemService.DeleteAsync(item);
                    //}

                    IMaterialRequestNoteService materialsRequestNoteService = ServiceProvider.GetService<IMaterialRequestNoteService>();
                    //MaterialsRequestNote_ItemService materialsRequestNote_ItemService = ServiceProvider.GetService<MaterialsRequestNote_ItemService>();
                    //materialsRequestNoteService.Username = Username;
                    //materialsRequestNoteService.Token = Token;
                    //materialsRequestNote_ItemService.Username = Username;

                    foreach (MaterialDistributionNoteItem materialDistributionNoteItem in materialDistributionNote.MaterialDistributionNoteItems)
                    {
                        MaterialsRequestNote materialsRequestNote = await materialsRequestNoteService.ReadByIdAsync(materialDistributionNoteItem.MaterialRequestNoteId);
                        materialsRequestNote.IsDistributed = true;

                        //inventory summary data

                        if (materialDistributionNote.Type.ToUpper().Equals("PRODUKSI"))
                        {
                            foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
                            {
                                materialsRequestNote.MaterialsRequestNote_Items.Where(w => w.ProductionOrderId.Equals(materialDistributionNoteDetail.ProductionOrderId)).Select(s => { s.DistributedLength -= materialDistributionNoteDetail.ReceivedLength; return s; }).ToList();


                            }
                            materialsRequestNoteService.UpdateDistributedQuantity(materialsRequestNote.Id, materialsRequestNote);

                            //foreach (MaterialsRequestNote_Item materialRequestNoteItem in materialsRequestNote.MaterialsRequestNote_Items)
                            //{
                            //    materialsRequestNote_ItemService.OnUpdating(materialRequestNoteItem.Id, materialRequestNoteItem);
                            //    materialsRequestNote_ItemService.DbSet.Update(materialRequestNoteItem);
                            //}
                        }
                        await materialsRequestNoteService.UpdateAsync(materialsRequestNote.Id, materialsRequestNote);
                        //materialsRequestNoteService.OnUpdating(materialsRequestNote.Id, materialsRequestNote);
                        //materialsRequestNoteService.DbSet.Update(materialsRequestNote);
                    }


                    await CreateInventoryDocument(materialDistributionNote, "IN");
                    Transaction.Commit();
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    throw ex;
                }
            }

            return Count;
        }

        public List<MaterialDistributionNoteReportViewModel> GetPdfReport(string unitId, string unitName, string type, DateTime date, int offset)
        {
            var Data = GetReportQuery(unitId, type, date, offset).ToList();

            return Data;
        }

        public Tuple<List<MaterialDistributionNoteReportViewModel>, int> GetReport(string unitId, string type, DateTime date, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(unitId, type, date, offset);

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

            Pageable<MaterialDistributionNoteReportViewModel> pageable = new Pageable<MaterialDistributionNoteReportViewModel>(Query, page - 1, size);
            List<MaterialDistributionNoteReportViewModel> Data = pageable.Data.ToList<MaterialDistributionNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MaterialDistributionNote MapToModel(MaterialDistributionNoteViewModel viewModel)
        {
            MaterialDistributionNote model = new MaterialDistributionNote();
            PropertyCopier<MaterialDistributionNoteViewModel, MaterialDistributionNote>.Copy(viewModel, model);

            model.UnitId = viewModel.Unit.Id.ToString();
            model.UnitCode = viewModel.Unit.Code;
            model.UnitName = viewModel.Unit.Name;

            model.MaterialDistributionNoteItems = new List<MaterialDistributionNoteItem>();
            foreach (MaterialDistributionNoteItemViewModel mdni in viewModel.MaterialDistributionNoteItems)
            {
                MaterialDistributionNoteItem materialDistributionNoteItem = new MaterialDistributionNoteItem();
                PropertyCopier<MaterialDistributionNoteItemViewModel, MaterialDistributionNoteItem>.Copy(mdni, materialDistributionNoteItem);

                materialDistributionNoteItem.MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetail>();
                foreach (MaterialDistributionNoteDetailViewModel mdnd in mdni.MaterialDistributionNoteDetails)
                {
                    MaterialDistributionNoteDetail materialDistributionNoteDetail = new MaterialDistributionNoteDetail();
                    PropertyCopier<MaterialDistributionNoteDetailViewModel, MaterialDistributionNoteDetail>.Copy(mdnd, materialDistributionNoteDetail);

                    materialDistributionNoteDetail.ProductionOrderId = mdnd.ProductionOrder.Id;
                    materialDistributionNoteDetail.ProductionOrderNo = mdnd.ProductionOrder.OrderNo;
                    materialDistributionNoteDetail.ProductionOrderIsCompleted = mdnd.ProductionOrder.IsCompleted;
                    materialDistributionNoteDetail.DistributedLength = mdnd.DistributedLength.GetValueOrDefault();

                    materialDistributionNoteDetail.ProductId = mdnd.Product.Id;
                    materialDistributionNoteDetail.ProductCode = mdnd.Product.Code;
                    materialDistributionNoteDetail.ProductName = mdnd.Product.Name;

                    materialDistributionNoteDetail.SupplierId = mdnd.Supplier._id;
                    materialDistributionNoteDetail.SupplierCode = mdnd.Supplier.code;
                    materialDistributionNoteDetail.SupplierName = mdnd.Supplier.name;

                    materialDistributionNoteItem.MaterialDistributionNoteDetails.Add(materialDistributionNoteDetail);
                }

                model.MaterialDistributionNoteItems.Add(materialDistributionNoteItem);
            }

            return model;
        }

        public MaterialDistributionNoteViewModel MapToViewModel(MaterialDistributionNote model)
        {
            MaterialDistributionNoteViewModel viewModel = new MaterialDistributionNoteViewModel();
            PropertyCopier<MaterialDistributionNote, MaterialDistributionNoteViewModel>.Copy(model, viewModel);

            UnitViewModel Unit = new UnitViewModel()
            {
                Id = model.UnitId,
                Code = model.UnitCode,
                Name = model.UnitName
            };

            viewModel.Unit = Unit;

            viewModel.MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>();
            if (model.MaterialDistributionNoteItems != null)
            {
                foreach (MaterialDistributionNoteItem mdni in model.MaterialDistributionNoteItems)
                {
                    MaterialDistributionNoteItemViewModel materialDistributionNoteItemViewModel = new MaterialDistributionNoteItemViewModel();
                    PropertyCopier<MaterialDistributionNoteItem, MaterialDistributionNoteItemViewModel>.Copy(mdni, materialDistributionNoteItemViewModel);

                    materialDistributionNoteItemViewModel.MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetailViewModel>();
                    foreach (MaterialDistributionNoteDetail mdnd in mdni.MaterialDistributionNoteDetails)
                    {
                        MaterialDistributionNoteDetailViewModel materialDistributionNoteDetailViewModel = new MaterialDistributionNoteDetailViewModel();
                        PropertyCopier<MaterialDistributionNoteDetail, MaterialDistributionNoteDetailViewModel>.Copy(mdnd, materialDistributionNoteDetailViewModel);

                        ProductionOrderViewModel productionOrder = new ProductionOrderViewModel
                        {
                            Id = mdnd.ProductionOrderId,
                            OrderNo = mdnd.ProductionOrderNo,
                            IsCompleted = mdnd.ProductionOrderIsCompleted,
                        };

                        ProductViewModel product = new ProductViewModel
                        {
                            Id = mdnd.ProductId,
                            Code = mdnd.ProductCode,
                            Name = mdnd.ProductName
                        };

                        SupplierViewModel supplier = new SupplierViewModel
                        {
                            _id = mdnd.SupplierId,
                            code = mdnd.SupplierCode,
                            name = mdnd.SupplierName
                        };

                        materialDistributionNoteDetailViewModel.ProductionOrder = productionOrder;
                        materialDistributionNoteDetailViewModel.Product = product;
                        materialDistributionNoteDetailViewModel.Supplier = supplier;

                        materialDistributionNoteItemViewModel.MaterialDistributionNoteDetails.Add(materialDistributionNoteDetailViewModel);
                    }

                    viewModel.MaterialDistributionNoteItems.Add(materialDistributionNoteItemViewModel);
                }
            }

            return viewModel;
        }

        public ReadResponse<MaterialDistributionNote> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<MaterialDistributionNote> Query = this.DbContext.MaterialDistributionNotes;

            List<string> SearchAttributes = new List<string>()
            {
                "No"
            };

            Query = QueryHelper<MaterialDistributionNote>.Search(Query, SearchAttributes, keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "No", "_CreatedUtc", "Type", "IsDisposition", "IsApproved"
            };

            Query = Query
                .Select(mdn => new MaterialDistributionNote
                {
                    Id = mdn.Id,
                    No = mdn.No,
                    _CreatedUtc = mdn._CreatedUtc,
                    Type = mdn.Type,
                    IsDisposition = mdn.IsDisposition,
                    IsApproved = mdn.IsApproved,
                    _LastModifiedUtc = mdn._LastModifiedUtc
                });

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<MaterialDistributionNote>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<MaterialDistributionNote>.Order(Query, OrderDictionary);

            Pageable<MaterialDistributionNote> pageable = new Pageable<MaterialDistributionNote>(Query, page - 1, size);
            List<MaterialDistributionNote> Data = pageable.Data.ToList<MaterialDistributionNote>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<MaterialDistributionNote>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<MaterialDistributionNote> ReadByIdAsync(int id)
        {
            var Data = await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.MaterialDistributionNoteItems)
                    .ThenInclude(d => d.MaterialDistributionNoteDetails)
                .FirstOrDefaultAsync();

            if (Data != null)
            {
                List<int> DetailsId = new List<int>();

                foreach (MaterialDistributionNoteItem item in Data.MaterialDistributionNoteItems)
                {
                    foreach (MaterialDistributionNoteDetail detail in item.MaterialDistributionNoteDetails)
                    {
                        DetailsId.Add(detail.MaterialsRequestNoteItemId);
                    }
                }

                var RequestNoteItems = this.DbContext.MaterialsRequestNote_Items.Select(p => new { Id = p.Id, IsCompleted = p.ProductionOrderIsCompleted }).Where(p => DetailsId.Contains(p.Id));

                foreach (MaterialDistributionNoteItem item in Data.MaterialDistributionNoteItems)
                {
                    foreach (MaterialDistributionNoteDetail detail in item.MaterialDistributionNoteDetails)
                    {
                        var RequestNoteItem = RequestNoteItems.FirstOrDefault(p => p.Id == detail.MaterialsRequestNoteItemId);
                        if (RequestNoteItem != null)
                            detail.IsCompleted = RequestNoteItem.IsCompleted;
                    }
                }
            }

            return Data;
        }

        public async Task<int> UpdateAsync(int id, MaterialDistributionNote model)
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
                    dbModel.IsDisposition = model.IsDisposition;
                    dbModel.AutoIncrementNumber = model.AutoIncrementNumber;
                    dbModel.No = model.No;
                    dbModel.Type = model.Type;
                    dbModel.UnitCode = model.UnitCode;
                    dbModel.UnitId = model.UnitId;
                    dbModel.UnitName = model.UnitName;
                    dbModel.FlagForUpdate(IdentityService.Username, UserAgent);
                    //DbSet.Update(dbModel);

                    var deletedItems = dbModel.MaterialDistributionNoteItems.Where(x => !model.MaterialDistributionNoteItems.Any(y => x.Id == y.Id));
                    var updatedItems = dbModel.MaterialDistributionNoteItems.Where(x => model.MaterialDistributionNoteItems.Any(y => x.Id == y.Id));
                    var addedItems = model.MaterialDistributionNoteItems.Where(x => !dbModel.MaterialDistributionNoteItems.Any(y => y.Id == x.Id));

                    foreach (var item in deletedItems)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                        foreach (var detail in item.MaterialDistributionNoteDetails)
                        {
                            detail.FlagForDelete(IdentityService.Username, UserAgent);
                        }

                    }

                    foreach (var item in updatedItems)
                    {
                        var selectedItem = model.MaterialDistributionNoteItems.FirstOrDefault(x => x.Id == item.Id);

                        item.MaterialRequestNoteCode = selectedItem.MaterialRequestNoteCode;
                        item.MaterialRequestNoteCreatedDateUtc = selectedItem.MaterialRequestNoteCreatedDateUtc;
                        item.MaterialRequestNoteId = selectedItem.MaterialRequestNoteId;
                        item.FlagForUpdate(IdentityService.Username, UserAgent);

                        var deletedDetails = item.MaterialDistributionNoteDetails.Where(x => !selectedItem.MaterialDistributionNoteDetails.Any(y => x.Id == y.Id));
                        var updatedDetails = item.MaterialDistributionNoteDetails.Where(x => selectedItem.MaterialDistributionNoteDetails.Any(y => x.Id == y.Id));
                        var addedDetails = selectedItem.MaterialDistributionNoteDetails.Where(x => !item.MaterialDistributionNoteDetails.Any(y => y.Id == x.Id));

                        foreach (var detail in deletedDetails)
                        {
                            item.FlagForDelete(IdentityService.Username, UserAgent);

                        }

                        foreach (var detail in updatedDetails)
                        {
                            var selectedDetail = selectedItem.MaterialDistributionNoteDetails.FirstOrDefault(x => x.Id == detail.Id);

                            detail.DistributedLength = selectedDetail.DistributedLength;
                            detail.Grade = selectedDetail.Grade;
                            detail.IsCompleted = selectedDetail.IsCompleted;
                            detail.IsDisposition = selectedDetail.IsDisposition;
                            detail.MaterialRequestNoteItemLength = selectedDetail.MaterialRequestNoteItemLength;
                            detail.MaterialsRequestNoteItemId = selectedDetail.MaterialsRequestNoteItemId;
                            detail.ProductCode = selectedDetail.ProductCode;
                            detail.ProductId = selectedDetail.ProductId;
                            detail.ProductionOrderId = selectedDetail.ProductionOrderId;
                            detail.ProductionOrderIsCompleted = selectedDetail.ProductionOrderIsCompleted;
                            detail.ProductionOrderNo = selectedDetail.ProductionOrderNo;
                            detail.ProductName = selectedDetail.ProductName;
                            detail.Quantity = selectedDetail.Quantity;
                            detail.ReceivedLength = selectedDetail.ReceivedLength;
                            detail.SupplierCode = selectedDetail.SupplierCode;
                            detail.SupplierId = selectedDetail.SupplierId;
                            detail.SupplierName = selectedDetail.SupplierName;
                            detail.FlagForUpdate(IdentityService.Username, UserAgent);
                        }


                        foreach (var detail in addedDetails)
                        {
                            detail.MaterialDistributionNoteItemId = id;
                            detail.FlagForCreate(IdentityService.Username, UserAgent);
                            detail.FlagForUpdate(IdentityService.Username, UserAgent);

                        }
                    }

                    foreach (var item in addedItems)
                    {
                        item.MaterialDistributionNoteId = id;
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        foreach (var detail in item.MaterialDistributionNoteDetails)
                        {
                            detail.FlagForCreate(IdentityService.Username, UserAgent);
                            detail.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
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

        public bool UpdateIsApprove(List<int> Ids)
        {
            bool IsSuccessful = false;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var mdn = this.DbSet.Where(m => Ids.Contains(m.Id)).ToList();
                    mdn.ForEach(m => { m.IsApproved = true; m._LastModifiedUtc = DateTime.UtcNow; m._LastModifiedAgent = UserAgent; m._LastModifiedBy = IdentityService.Username; });
                    this.DbContext.SaveChanges();

                    IsSuccessful = true;
                    Transaction.Commit();
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    throw ex;
                }
            }

            return IsSuccessful;
        }

        public async Task<int> CreateInventoryDocument(MaterialDistributionNote Model, string Type)
        {
            string storageURI = "master/storages";
            string uomURI = "master/uoms";

            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));
            /* Get UOM */
            Dictionary<string, object> filterUOM = new Dictionary<string, object> { { "unit", "MTR" } };
            var responseUOM = httpClient.GetAsync($@"{APIEndpoint.Core}{uomURI}?filter=" + JsonConvert.SerializeObject(filterUOM)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultUOM = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseUOM.Result);
            var jsonUOM = resultUOM.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> uom = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonUOM.ToString())[0];

            /* Get Storage */
            var storageName = Model.UnitName.Equals("PRINTING") ? "Gudang Greige Printing" : "Gudang Greige Finishing";
            Dictionary<string, object> filterStorage = new Dictionary<string, object> { { "name", storageName } };
            var responseStorage = httpClient.GetAsync($@"{APIEndpoint.Core}{storageURI}?filter=" + JsonConvert.SerializeObject(filterStorage)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultStorage = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStorage.Result);
            var jsonStorage = resultStorage.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> storage = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonStorage.ToString())[0];

            /* Create Inventory Document */
            List<InventoryDocumentItem> inventoryDocumentItems = new List<InventoryDocumentItem>();
            List<MaterialDistributionNoteDetail> mdnds = new List<MaterialDistributionNoteDetail>();

            foreach (MaterialDistributionNoteItem mdni in Model.MaterialDistributionNoteItems)
            {
                mdnds.AddRange(mdni.MaterialDistributionNoteDetails);
            }

            //List<MaterialDistributionNoteDetail> list = mdnds
            //        .GroupBy(m => new { m.ProductId, m.ProductCode, m.ProductName })
            //        .Select(s => new MaterialDistributionNoteDetail
            //        {
            //            ProductId = s.First().ProductId,
            //            ProductCode = s.First().ProductCode,
            //            ProductName = s.First().ProductName,
            //            ReceivedLength = s.Sum(d => d.ReceivedLength),
            //            MaterialRequestNoteItemLength = s.Sum(d => d.MaterialRequestNoteItemLength)
            //        }).ToList();

            foreach (MaterialDistributionNoteDetail mdnd in mdnds)
            {
                InventoryDocumentItem inventoryDocumentItem = new InventoryDocumentItem
                {
                    ProductId = int.Parse(mdnd.ProductId),
                    ProductCode = mdnd.ProductCode,
                    ProductName = mdnd.ProductName,
                    Quantity = mdnd.ReceivedLength,
                    StockPlanning = Model.Type != "RE-GRADING" ? (mdnd.DistributedLength == 0 ? mdnd.MaterialRequestNoteItemLength - mdnd.ReceivedLength : mdnd.ReceivedLength * -1) : mdnd.ReceivedLength * -1,
                    UomId = int.Parse(uom["Id"].ToString()),
                    UomUnit = uom["Unit"].ToString()
                };

                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            List<InventoryDocumentItem> list = inventoryDocumentItems
                    .GroupBy(m => new { m.ProductId, m.ProductCode, m.ProductName })
                    .Select(s => new InventoryDocumentItem
                    {
                        ProductId = s.First().ProductId,
                        ProductCode = s.First().ProductCode,
                        ProductName = s.First().ProductName,
                        Quantity = s.Sum(d => d.Quantity),
                        StockPlanning = s.Sum(d => d.StockPlanning),
                        UomUnit = s.First().UomUnit,
                        UomId = s.First().UomId
                    }).ToList();

            InventoryDocument inventoryDocument = new InventoryDocument
            {
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Model.No,
                ReferenceType = "Bon Pengantar Greige",
                Type = Type,
                StorageId = int.Parse(storage["_id"].ToString()),
                StorageCode = storage["code"].ToString(),
                StorageName = storage["name"].ToString(),
                Items = list
            };

            var inventoryDocumentFacade = ServiceProvider.GetService<IInventoryDocumentService>();
            return await inventoryDocumentFacade.Create(inventoryDocument);
        }

        public async Task<MaterialDistributionNote> CustomCodeGenerator(MaterialDistributionNote Model)
        {
            var Type = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "PR" : "FS";
            var lastData = await this.DbSet.Where(w => w.UnitCode == Model.UnitCode).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.No = $"P{Type}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"P{Type}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"P{Type}{Year}{Number}";
                }
            }

            return Model;
        }

        public IQueryable<MaterialDistributionNoteReportViewModel> GetReportQuery(string unitId, string type, DateTime date, int offset)
        {
            var Query = (from a in DbContext.MaterialDistributionNotes
                         join b in DbContext.MaterialDistributionNoteItems on a.Id equals b.MaterialDistributionNoteId
                         join c in DbContext.MaterialDistributionNoteDetails on b.Id equals c.MaterialDistributionNoteItemId
                         where a._IsDeleted == false
                             && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                             && a.Type == (string.IsNullOrWhiteSpace(type) ? a.Type : type)
                             && a._CreatedUtc.AddHours(offset).Date == date.Date
                         select new MaterialDistributionNoteReportViewModel
                         {
                             _LastModifiedUtc = a._LastModifiedUtc,
                             No = a.No,
                             Type = a.Type,
                             MaterialRequestNoteNo = b.MaterialRequestNoteCode,
                             ProductionOrderNo = c.ProductionOrderNo,
                             ProductName = c.ProductName,
                             Grade = c.Grade,
                             Quantity = c.Quantity,
                             Length = c.ReceivedLength,
                             SupplierName = c.SupplierName,
                             IsDisposition = a.IsDisposition,
                             IsApproved = a.IsApproved
                         });

            return Query;
        }

        //public async Task<int> UpdateIsCompleted(int Id, MaterialDistributionNote Model)
        //{
        //    int IsSucceed = 0;

        //    using (var Transaction = this.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            IsSucceed = await UpdateModel(Id, Model);
        //            MaterialDistributionNoteItemService materialDistributionNoteItemService = ServiceProvider.GetService<MaterialDistributionNoteItemService>();
        //            MaterialDistributionNoteDetailService materialDistributionNoteDetailService = ServiceProvider.GetService<MaterialDistributionNoteDetailService>();
        //            MaterialsRequestNoteService materialsRequestNoteService = ServiceProvider.GetService<MaterialsRequestNoteService>();

        //            materialDistributionNoteItemService.Username = this.Username;
        //            materialDistributionNoteDetailService.Username = this.Username;
        //            materialsRequestNoteService.Username = Username;
        //            materialsRequestNoteService.Token = Token;

        //            foreach (MaterialDistributionNoteItem materialDistributionNoteItem in Model.MaterialDistributionNoteItems)
        //            {
        //                MaterialsRequestNote materialsRequestNote = await materialsRequestNoteService.ReadModelById(materialDistributionNoteItem.MaterialRequestNoteId);
        //                await materialDistributionNoteItemService.UpdateModel(materialDistributionNoteItem.Id, materialDistributionNoteItem);

        //                foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
        //                {
        //                    await materialDistributionNoteDetailService.UpdateModel(materialDistributionNoteDetail.Id, materialDistributionNoteDetail);
        //                    if (materialDistributionNoteDetail.IsCompleted)
        //                    {
        //                        materialsRequestNote.MaterialsRequestNote_Items.Where(w => w.ProductionOrderId.Equals(materialDistributionNoteDetail.ProductionOrderId)).Select(s => { s.ProductionOrderIsCompleted = true; return s; }).ToList();
        //                    }
        //                }

        //                await materialsRequestNoteService.UpdateIsComplete(materialsRequestNote.Id, materialsRequestNote);

        //            }

        //            Transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            Transaction.Rollback();
        //        }
        //    }

        //    return IsSucceed;
        //}
    }
}
