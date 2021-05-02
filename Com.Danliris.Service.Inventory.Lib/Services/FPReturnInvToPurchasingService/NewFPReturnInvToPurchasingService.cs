using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService
{
    public class NewFPReturnInvToPurchasingService : IFPReturnInvToPurchasingService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<FPReturnInvToPurchasing> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public NewFPReturnInvToPurchasingService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<FPReturnInvToPurchasing>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(FPReturnInvToPurchasing model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    IFpRegradingResultDocsService fpRegradingResultDocsService = ServiceProvider.GetService<IFpRegradingResultDocsService>();
                    foreach (FPReturnInvToPurchasingDetail detail in model.FPReturnInvToPurchasingDetails)
                    {
                        fpRegradingResultDocsService.UpdateIsReturnedToPurchasing(detail.FPRegradingResultDocsId, true);
                        detail.FlagForCreate(IdentityService.Username, UserAgent);
                        detail.FlagForUpdate(IdentityService.Username, UserAgent);
                    }

                    model = await this.NoGenerator(model);
                    DbSet.Add(model);
                    Created = await DbContext.SaveChangesAsync();
                    await CreateInventoryDocumentAsync(model, "OUT");

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Insert Error : " + e.Message);
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
                    IFpRegradingResultDocsService fpRegradingResultDocsService = ServiceProvider.GetService<IFpRegradingResultDocsService>();
                    FPReturnInvToPurchasing fpReturnInvToPurchasing = await ReadByIdAsync(id);
                    fpReturnInvToPurchasing.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in fpReturnInvToPurchasing.FPReturnInvToPurchasingDetails)
                    {
                        fpRegradingResultDocsService.UpdateIsReturnedToPurchasing(item.FPRegradingResultDocsId, false);
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }
                    Count = await DbContext.SaveChangesAsync();
                    

                    await CreateInventoryDocumentAsync(fpReturnInvToPurchasing, "IN");

                    Transaction.Commit();
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    throw new Exception("Delete Error : " + e.Message);
                }
            }

            return Count;
        }

        public FPReturnInvToPurchasing MapToModel(FPReturnInvToPurchasingViewModel viewModel)
        {
            FPReturnInvToPurchasing model = new FPReturnInvToPurchasing();
            PropertyCopier<FPReturnInvToPurchasingViewModel, FPReturnInvToPurchasing>.Copy(viewModel, model);

            #region Unit

            model.UnitName = viewModel.Unit.Name;

            #endregion Unit

            #region Supplier

            model.SupplierId = viewModel.Supplier._id;
            model.SupplierCode = viewModel.Supplier.code;
            model.SupplierName = viewModel.Supplier.name;

            #endregion Supplier

            model.FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetail>();
            foreach (FPReturnInvToPurchasingDetailViewModel detailVM in viewModel.FPReturnInvToPurchasingDetails)
            {
                FPReturnInvToPurchasingDetail detail = new FPReturnInvToPurchasingDetail();
                PropertyCopier<FPReturnInvToPurchasingDetailViewModel, FPReturnInvToPurchasingDetail>.Copy(detailVM, detail);

                #region Product

                detail.ProductId = detailVM.Product.Id;
                detail.ProductCode = detailVM.Product.Code;
                detail.ProductName = detailVM.Product.Name;

                #endregion Product
                
                model.FPReturnInvToPurchasingDetails.Add(detail);
            }

            return model;
        }

        public FPReturnInvToPurchasingViewModel MapToViewModel(FPReturnInvToPurchasing model)
        {
            FPReturnInvToPurchasingViewModel viewModel = new FPReturnInvToPurchasingViewModel();
            PropertyCopier<FPReturnInvToPurchasing, FPReturnInvToPurchasingViewModel>.Copy(model, viewModel);

            #region Unit

            viewModel.Unit = new UnitViewModel();
            viewModel.Unit.Name = model.UnitName;

            #endregion Unit

            #region Supplier

            viewModel.Supplier = new SupplierViewModel();
            viewModel.Supplier._id = model.SupplierId;
            viewModel.Supplier.code = model.SupplierCode;
            viewModel.Supplier.name = model.SupplierName;

            #endregion Supplier

            viewModel.FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>();
            foreach (FPReturnInvToPurchasingDetail detail in model.FPReturnInvToPurchasingDetails)
            {
                FPReturnInvToPurchasingDetailViewModel detailVM = new FPReturnInvToPurchasingDetailViewModel();
                PropertyCopier<FPReturnInvToPurchasingDetail, FPReturnInvToPurchasingDetailViewModel>.Copy(detail, detailVM);

                #region Product

                detailVM.Product = new ProductViewModel();
                detailVM.Product.Id = detail.ProductId;
                detailVM.Product.Code = detail.ProductCode;
                detailVM.Product.Name = detail.ProductName;

                #endregion Product
                viewModel.FPReturnInvToPurchasingDetails.Add(detailVM);
            }
            return viewModel;
        }

        public Tuple<List<object>, int, Dictionary<string, string>> Read(int page, int size, string order, string keyword, string filter)
        {
            #region Query

            IQueryable<FPReturnInvToPurchasing> Query = this.DbSet;

            Query = Query
                .Select(s => new FPReturnInvToPurchasing
                {
                    Id = s.Id,
                    No = s.No,
                    UnitName = s.UnitName,
                    SupplierName = s.SupplierName,
                    _CreatedUtc = s._CreatedUtc,
                    _LastModifiedUtc = s._LastModifiedUtc,
                    FPReturnInvToPurchasingDetails = s.FPReturnInvToPurchasingDetails.Select(p => new FPReturnInvToPurchasingDetail { FPReturnInvToPurchasingId = p.FPReturnInvToPurchasingId, Quantity = p.Quantity, Length = p.Length, NecessaryLength = p.NecessaryLength }).Where(i => i.FPReturnInvToPurchasingId.Equals(s.Id)).ToList()
                });

            #endregion Query

            #region Search

            List<string> searchAttributes = new List<string>()
            {
                "No"
            };

            Query = QueryHelper<FPReturnInvToPurchasing>.Search(Query, searchAttributes, keyword);

            #endregion Search

            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<FPReturnInvToPurchasing>.Order(Query, OrderDictionary);

            #endregion OrderBy

            #region Paging

            Pageable<FPReturnInvToPurchasing> pageable = new Pageable<FPReturnInvToPurchasing>(Query, page - 1, size);
            List<FPReturnInvToPurchasing> Data = pageable.Data.ToList<FPReturnInvToPurchasing>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            List<object> list = new List<object>();
            list.AddRange(
                Data
                    .GroupBy(d => new { d.Id, d.No, d.UnitName, d._CreatedUtc, d.SupplierName })
                    .Select(s => new
                    {
                        Id = s.First().Id,
                        No = s.First().No,
                        UnitName = s.First().UnitName,
                        TotalQuantity = s.Sum(d => d.FPReturnInvToPurchasingDetails.Sum(p => p.Quantity)),
                        TotalLength = s.Sum(d => d.FPReturnInvToPurchasingDetails.Sum(p => p.Length)),
                        TotalNecessaryLength = s.Sum(d => d.FPReturnInvToPurchasingDetails.Sum(p => p.NecessaryLength)),
                        SupplierName = s.First().SupplierName,
                        _CreatedUtc = s.First()._CreatedUtc
                    }).ToList()
            );

            return Tuple.Create(list, TotalData, OrderDictionary);
        }

        public ReadResponse<FPReturnInvToPurchasing> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            #region Query

            IQueryable<FPReturnInvToPurchasing> Query = this.DbSet;
            List<string> SelectedFields = new List<string>()
                {
                    "Id", "No", "Unit", "Supplier", "FPReturnInvToPurchasingDetails", "_CreatedUtc", "_LastModifiedUtc"
                };
            Query = Query
                .Select(s => new FPReturnInvToPurchasing
                {
                    Id = s.Id,
                    No = s.No,
                    UnitName = s.UnitName,
                    SupplierName = s.SupplierName,
                    _CreatedUtc = s._CreatedUtc,
                    _LastModifiedUtc = s._LastModifiedUtc,
                    FPReturnInvToPurchasingDetails = s.FPReturnInvToPurchasingDetails.Select(p => new FPReturnInvToPurchasingDetail { FPReturnInvToPurchasingId = p.FPReturnInvToPurchasingId, Quantity = p.Quantity, Length = p.Length, NecessaryLength = p.NecessaryLength }).Where(i => i.FPReturnInvToPurchasingId.Equals(s.Id)).ToList()
                });

            #endregion Query

            #region Search

            List<string> searchAttributes = new List<string>()
            {
                "No"
            };

            Query = QueryHelper<FPReturnInvToPurchasing>.Search(Query, searchAttributes, keyword);

            #endregion Search

            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<FPReturnInvToPurchasing>.Order(Query, OrderDictionary);

            #endregion OrderBy

            #region Paging

            Pageable<FPReturnInvToPurchasing> pageable = new Pageable<FPReturnInvToPurchasing>(Query, page - 1, size);
            List<FPReturnInvToPurchasing> Data = pageable.Data.ToList<FPReturnInvToPurchasing>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            return new ReadResponse<FPReturnInvToPurchasing>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<FPReturnInvToPurchasing> ReadByIdAsync(int id)
        {
            return await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.FPReturnInvToPurchasingDetails)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, FPReturnInvToPurchasing model)
        {
            int updated = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (id != model.Id)
                        throw new Exception("data not found");

                    var dbModel = await ReadByIdAsync(id);
                    
                    dbModel.SupplierCode = model.SupplierCode;
                    dbModel.SupplierName = model.SupplierName;
                    dbModel.SupplierId = model.SupplierId;
                    dbModel.UnitName = model.UnitName;

                    dbModel.FlagForUpdate(IdentityService.Username, UserAgent);
                    //DbSet.Update(dbModel);

                    var deletedItems = dbModel.FPReturnInvToPurchasingDetails.Where(x => !model.FPReturnInvToPurchasingDetails.Any(y => x.Id == y.Id));
                    var updatedItems = dbModel.FPReturnInvToPurchasingDetails.Where(x => model.FPReturnInvToPurchasingDetails.Any(y => x.Id == y.Id));
                    var addedItems = model.FPReturnInvToPurchasingDetails.Where(x => !dbModel.FPReturnInvToPurchasingDetails.Any(y => y.Id == x.Id));

                    foreach (var item in deletedItems)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);

                    }

                    foreach (var item in updatedItems)
                    {
                        var selectedItem = model.FPReturnInvToPurchasingDetails.FirstOrDefault(x => x.Id == item.Id);
                        
                        item.Length = selectedItem.Length;
                        item.ProductCode = selectedItem.ProductCode;
                        item.ProductId = selectedItem.ProductId;
                        item.ProductName = selectedItem.ProductName;
                        item.Quantity = selectedItem.Quantity;
                        item.Description = selectedItem.Description;
                        item.FPRegradingResultDocsCode = selectedItem.FPRegradingResultDocsCode;
                        item.FPRegradingResultDocsId = selectedItem.FPRegradingResultDocsId;
                        item.NecessaryLength = selectedItem.NecessaryLength;
                        
                        item.FlagForUpdate(IdentityService.Username, UserAgent);

                    }

                    foreach (var item in addedItems)
                    {
                        item.FPReturnInvToPurchasingId = id;
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

        public async Task<FPReturnInvToPurchasing> NoGenerator(FPReturnInvToPurchasing Model)
        {
            var unit = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "PR" : "FS";
            var lastData = await this.DbSet.Where(w => w.UnitName == Model.UnitName).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.No = $"BL{unit}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"BL{unit}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"BL{unit}{Year}{Number}";
                }
            }

            return Model;
        }

        public async Task<int> CreateInventoryDocumentAsync(FPReturnInvToPurchasing model, string Type)
        {
            string storageURI = "master/storages";
            string uomURI = "master/uoms";

            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));
            #region UOM

            Dictionary<string, object> filterUOM = new Dictionary<string, object> { { "unit", "MTR" } };
            var responseUOM = httpClient.GetAsync($@"{APIEndpoint.Core}{uomURI}?filter=" + JsonConvert.SerializeObject(filterUOM)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultUOM = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseUOM.Result);
            var jsonUOM = resultUOM.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> uom = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonUOM.ToString())[0];

            #endregion UOM

            #region Storage

            var storageName = model.UnitName.Equals("PRINTING") ? "Gudang Greige Printing" : "Gudang Greige Finishing";
            Dictionary<string, object> filterStorage = new Dictionary<string, object> { { "name", storageName } };
            var responseStorage = httpClient.GetAsync($@"{APIEndpoint.Core}{storageURI}?filter=" + JsonConvert.SerializeObject(filterStorage)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultStorage = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStorage.Result);
            var jsonStorage = resultStorage.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> storage = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonStorage.ToString())[0];

            #endregion Storage

            #region Inventory Document

            List<InventoryDocumentItem> inventoryDocumentItems = new List<InventoryDocumentItem>();

            foreach (FPReturnInvToPurchasingDetail detail in model.FPReturnInvToPurchasingDetails)
            {
                InventoryDocumentItem inventoryDocumentItem = new InventoryDocumentItem
                {
                    ProductId = int.Parse(detail.ProductId),
                    ProductCode = detail.ProductCode,
                    ProductName = detail.ProductName,
                    Quantity = detail.Length,
                    UomId = int.Parse(uom["Id"].ToString()),
                    UomUnit = uom["Unit"].ToString()
                };

                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            InventoryDocument inventoryDocument = new InventoryDocument
            {
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = model.No,
                ReferenceType = "Bon Retur Barang - Pembelian",
                Type = Type,
                StorageId = int.Parse(storage["_id"].ToString()),
                StorageCode = storage["code"].ToString(),
                StorageName = storage["name"].ToString(),
                Items = inventoryDocumentItems
            };

            var inventoryDocumentFacade = ServiceProvider.GetService<IInventoryDocumentService>();
            return await inventoryDocumentFacade.Create(inventoryDocument);

            #endregion Inventory Document
        }
    }
}
