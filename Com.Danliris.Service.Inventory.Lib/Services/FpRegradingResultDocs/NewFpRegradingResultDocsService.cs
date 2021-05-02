using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FpRegradingResultDocs;
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

namespace Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs
{
    public class NewFpRegradingResultDocsService : IFpRegradingResultDocsService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<Models.FpRegradingResultDocs> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public NewFpRegradingResultDocsService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<Models.FpRegradingResultDocs>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(Models.FpRegradingResultDocs model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    model = await this.CustomCodeGenerator(model);
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);
                    foreach (var item in model.Details)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }

                    DbSet.Add(model);

                    Created = await DbContext.SaveChangesAsync();

                    await CreateInventoryDocumentAsync(model, "IN");

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
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
                    Models.FpRegradingResultDocs fpReturProInvDocs = await ReadByIdAsync(id);
                    fpReturProInvDocs.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in fpReturProInvDocs.Details)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }
                    Count = await DbContext.SaveChangesAsync();

                    //FpRegradingResultDetailsDocsService fpReturProInvDocsDetailsService = ServiceProvider.GetService<FpRegradingResultDetailsDocsService>();
                    //fpReturProInvDocsDetailsService.Username = this.Username;


                    //HashSet<int> fpReturProInvDocsDetails = new HashSet<int>(this.DbContext.fpRegradingResultDocsDetails.Where(p => p.FpReturProInvDocsId.Equals(Id)).Select(p => p.Id));

                    //foreach (int detail in fpReturProInvDocsDetails)
                    //{
                    //    await fpReturProInvDocsDetailsService.DeleteAsync(detail);
                    //}

                    await CreateInventoryDocumentAsync(fpReturProInvDocs, "OUT");

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

        public Models.FpRegradingResultDocs MapToModel(FpRegradingResultDocsViewModel viewModel)
        {
            Models.FpRegradingResultDocs model = new Models.FpRegradingResultDocs();
            PropertyCopier<FpRegradingResultDocsViewModel, Models.FpRegradingResultDocs>.Copy(viewModel, model);

            model.NoBon = viewModel.Bon.no;
            model.NoBonId = viewModel.Bon._id;
            model.UnitName = viewModel.Bon.unitName;
            model.SupplierId = viewModel.Supplier._id;
            model.SupplierName = viewModel.Supplier.name;
            model.SupplierCode = viewModel.Supplier.code;
            model.ProductId = viewModel.Product.Id;
            model.ProductCode = viewModel.Product.Code;
            model.ProductName = viewModel.Product.Name;
            model.Shift = viewModel.Shift;
            model.MachineName = viewModel.Machine.name;
            model.MachineId = viewModel.Machine._id;
            model.MachineCode = viewModel.Machine.code;
            model.Operator = viewModel.Operator;
            model.Remark = viewModel.Remark;

            model.Details = new List<FpRegradingResultDocsDetails>();

            foreach (FpRegradingResultDetailsDocsViewModel data in viewModel.Details)
            {
                FpRegradingResultDocsDetails detail = new FpRegradingResultDocsDetails
                {
                    //detail.SupplierId = viewModel.Supplier._id;
                    ProductId = viewModel.Product.Id,
                    ProductCode = viewModel.Product.Code,
                    ProductName = viewModel.Product.Name,
                    Quantity = data.Quantity,
                    Length = data.Length,
                    Remark = data.Remark,
                    Grade = data.Grade,
                    //detail.GradeBefore = data.GradeBefore;
                    Retur = data.Retur,
                    Id = data.Id
                };
                //detail.LengthBeforeReGrade = data.LengthBeforeReGrade;
                model.Details.Add(detail);

            }

            return model;
        }

        public FpRegradingResultDocsViewModel MapToViewModel(Models.FpRegradingResultDocs model)
        {
            FpRegradingResultDocsViewModel viewModel = new FpRegradingResultDocsViewModel();
            PropertyCopier<Models.FpRegradingResultDocs, FpRegradingResultDocsViewModel>.Copy(model, viewModel);

            viewModel.Details = new List<FpRegradingResultDetailsDocsViewModel>();
            viewModel.Bon = new FpRegradingResultDocsViewModel.noBon
            {
                _id = model.NoBonId,
                no = model.NoBon,
                unitName = model.UnitName
            };

            viewModel.Supplier = new FpRegradingResultDocsViewModel.supplier
            {
                name = model.SupplierName,
                _id = model.SupplierId,
                code = model.SupplierCode
            };
            viewModel._CreatedUtc = model._CreatedUtc;

            viewModel.Product = new FpRegradingResultDocsViewModel.product
            {
                Name = model.ProductName,
                Id = model.ProductId,
                Code = model.SupplierCode
            };

            viewModel.Shift = model.Shift;
            viewModel.Remark = model.Remark;
            viewModel.Operator = model.Operator;

            viewModel.Machine = new FpRegradingResultDocsViewModel.machine
            {
                name = model.MachineName,
                code = model.MachineCode,
                _id = model.MachineId
            };

            foreach (FpRegradingResultDocsDetails data in model.Details)
            {
                FpRegradingResultDetailsDocsViewModel detail = new FpRegradingResultDetailsDocsViewModel
                {
                    Product = new FpRegradingResultDetailsDocsViewModel.product()
                };
                detail.Product.Id = data.ProductId;
                detail.Product.Name = data.ProductName;
                detail.Product.Length = data.Length;
                //detail.Product.Quantity = data.Quantity;
                //detail.Quantity = data.Quantity;
                detail.Remark = data.Remark;
                detail.Quantity = data.Quantity;
                detail.Length = data.Length;
                //detail.GradeBefore = data.GradeBefore;
                detail.Grade = data.Grade;
                detail.Retur = data.Retur;
                detail.Id = data.Id;
                //detail.LengthBeforeReGrade = data.LengthBeforeReGrade;
                viewModel.Details.Add(detail);
            }
            return viewModel;
        }

        public ReadResponse<Models.FpRegradingResultDocs> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<Models.FpRegradingResultDocs> Query = this.DbContext.fpRegradingResultDocs;

            List<string> SearchAttributes = new List<string>()
                {
                    "Code", "NoBon", "SupplierName","ProductName"
                };
            Query = QueryHelper<Models.FpRegradingResultDocs>.Search(Query, SearchAttributes, keyword);

            List<string> SelectedFields = new List<string>()
                {
                    "Id", "Code", "Date", "Bon", "Supplier", "Product", "Details", "Machine", "Remark", "Operator", "IsReturnedToPurchasing"
                };
            Query = Query
                .Select(o => new Models.FpRegradingResultDocs
                {
                    Id = o.Id,
                    Code = o.Code,
                    Date = o.Date,
                    NoBon = o.NoBon,
                    NoBonId = o.NoBonId,
                    UnitName = o.UnitName,
                    SupplierId = o.SupplierId,
                    SupplierName = o.SupplierName,
                    SupplierCode = o.SupplierCode,
                    ProductId = o.ProductId,
                    ProductName = o.ProductName,
                    ProductCode = o.ProductCode,
                    Remark = o.Remark,
                    Operator = o.Operator,
                    MachineCode = o.MachineCode,
                    MachineId = o.MachineId,
                    MachineName = o.MachineName,
                    Shift = o.Shift,
                    _CreatedUtc = o._CreatedUtc,
                    _LastModifiedUtc = o._LastModifiedUtc,
                    IsReturnedToPurchasing = o.IsReturnedToPurchasing,
                    Details = o.Details.Select(p => new FpRegradingResultDocsDetails { FpReturProInvDocsId = p.FpReturProInvDocsId, ProductName = p.ProductName, ProductCode = p.ProductCode, ProductId = p.ProductId, Id = o.Id, Length = p.Length, Quantity = p.Quantity, Remark = p.Remark, Code = p.Code, Grade = p.Grade, Retur = p.Retur }).Where(i => i.FpReturProInvDocsId.Equals(o.Id)).ToList()
                });

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<Models.FpRegradingResultDocs>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<Models.FpRegradingResultDocs>.Order(Query, OrderDictionary);

            Pageable<Models.FpRegradingResultDocs> pageable = new Pageable<Models.FpRegradingResultDocs>(Query, page - 1, size);
            List<Models.FpRegradingResultDocs> Data = pageable.Data.ToList<Models.FpRegradingResultDocs>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<Models.FpRegradingResultDocs>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<Models.FpRegradingResultDocs> ReadByIdAsync(int id)
        {
            return await this.DbSet
               .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
               .Include(d => d.Details)
               .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, Models.FpRegradingResultDocs model)
        {
            int updated = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (id != model.Id)
                        throw new Exception("data not found");

                    var dbModel = await ReadByIdAsync(id);

                    dbModel.Date = model.Date;
                    dbModel.IsReturn = model.IsReturn;
                    dbModel.IsReturnedToPurchasing = model.IsReturnedToPurchasing;
                    dbModel.MachineCode = model.MachineCode;
                    dbModel.MachineId = model.MachineId;
                    dbModel.MachineName = model.MachineName;
                    dbModel.NoBon = model.NoBon;
                    dbModel.NoBonId = model.NoBonId;
                    dbModel.Operator = model.Operator;
                    dbModel.OriginalGrade = model.OriginalGrade;
                    dbModel.ProductCode = model.ProductCode;
                    dbModel.ProductId = model.ProductId;
                    dbModel.ProductName = model.ProductName;
                    dbModel.Remark = model.Remark;
                    dbModel.Shift = model.Shift;
                    dbModel.SupplierCode = model.SupplierCode;
                    dbModel.SupplierName = model.SupplierName;
                    dbModel.SupplierId = model.SupplierId;
                    dbModel.TotalLength = model.TotalLength;
                    dbModel.UnitName = model.UnitName;

                    dbModel.FlagForUpdate(IdentityService.Username, UserAgent);
                    //DbSet.Update(dbModel);

                    var deletedItems = dbModel.Details.Where(x => !model.Details.Any(y => x.Id == y.Id));
                    var updatedItems = dbModel.Details.Where(x => model.Details.Any(y => x.Id == y.Id));
                    var addedItems = model.Details.Where(x => !dbModel.Details.Any(y => y.Id == x.Id));

                    foreach (var item in deletedItems)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);

                    }

                    foreach (var item in updatedItems)
                    {
                        var selectedItem = model.Details.FirstOrDefault(x => x.Id == item.Id);

                        item.Grade = selectedItem.Grade;
                        item.Length = selectedItem.Length;
                        item.ProductCode = selectedItem.ProductCode;
                        item.ProductId = selectedItem.ProductId;
                        item.ProductName = selectedItem.ProductName;
                        item.Quantity = selectedItem.Quantity;
                        item.Remark = selectedItem.Remark;
                        item.Retur = selectedItem.Retur;
                        item.FlagForUpdate(IdentityService.Username, UserAgent);

                    }

                    foreach (var item in addedItems)
                    {
                        item.FpReturProInvDocsId = id;
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

        public async Task<int> CreateInventoryDocumentAsync(Models.FpRegradingResultDocs Model, string Type)
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
            InventoryDocumentItem inventoryDocumentItem = new InventoryDocumentItem();
            double TotalLength = 0;
            foreach (FpRegradingResultDocsDetails o in Model.Details)
            {
                TotalLength += o.Length;
            }

            inventoryDocumentItem.ProductId = int.Parse(Model.ProductId);
            inventoryDocumentItem.ProductCode = Model.ProductCode;
            inventoryDocumentItem.ProductName = Model.ProductName;
            inventoryDocumentItem.Quantity = TotalLength;
            inventoryDocumentItem.UomId = int.Parse(uom["Id"].ToString());
            inventoryDocumentItem.UomUnit = uom["Unit"].ToString();

            inventoryDocumentItems.Add(inventoryDocumentItem);

            InventoryDocument inventoryDocument = new InventoryDocument
            {
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Model.Code,
                ReferenceType = "Bon Hasil Re-grading",
                Type = Type,
                StorageId = int.Parse(storage["_id"].ToString()),
                StorageCode = storage["code"].ToString(),
                StorageName = storage["name"].ToString(),
                Items = inventoryDocumentItems
            };

            var inventoryDocumentFacade = ServiceProvider.GetService<IInventoryDocumentService>();
            return await inventoryDocumentFacade.Create(inventoryDocument);
        }

        public async Task<Models.FpRegradingResultDocs> CustomCodeGenerator(Models.FpRegradingResultDocs Model)
        {
            var unit = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "PR" : "FS";
            var lastData = await this.DbSet.Where(w => w.UnitName == Model.UnitName).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");
            //string Month = Now.ToString("MM");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.Code = $"KB{unit}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"KB{unit}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"KB{unit}{Year}{Number}";
                }
            }

            return Model;
        }

        public Tuple<List<Models.FpRegradingResultDocs>, int, Dictionary<string, string>, List<string>> ReadNo(string Keyword = null, string Filter = "{}", int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null)
        {
            //Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            //string unitName = FilterDictionary["UnitName"].ToString();
            //string supplierId = FilterDictionary["SupplierId"].ToString();
            IQueryable<Models.FpRegradingResultDocs> Query = this.DbContext.fpRegradingResultDocs.Where(p => p._IsDeleted == false && p.IsReturnedToPurchasing == false);

            List<string> SearchAttributes = new List<string>()
            {
                "Code"
            };
            Query = QueryHelper<Models.FpRegradingResultDocs>.Search(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code"
            };

            Query = Query
                .Select(o => new Models.FpRegradingResultDocs
                {
                    Id = o.Id,
                    Code = o.Code,
                    _LastModifiedUtc = o._LastModifiedUtc,
                    UnitName = o.UnitName,
                    SupplierId = o.SupplierId,
                    Details = o.Details

                });
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = QueryHelper<Models.FpRegradingResultDocs>.Filter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<Models.FpRegradingResultDocs>.Order(Query, OrderDictionary);

            Pageable<Models.FpRegradingResultDocs> pageable = new Pageable<Models.FpRegradingResultDocs>(Query, Page - 1, Size);
            List<Models.FpRegradingResultDocs> Data = pageable.Data.ToList<Models.FpRegradingResultDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public int UpdateIsReturnedToPurchasing(int fPRegradingResultDocsId, bool flag)
        {
            var model = DbContext.fpRegradingResultDocs.FirstOrDefault(x => x.Id == fPRegradingResultDocsId);
            //Models.FpRegradingResultDocs model = new Models.FpRegradingResultDocs { Id = fPRegradingResultDocsId, IsReturnedToPurchasing = flag };
            model.IsReturnedToPurchasing = flag;
            //DbContext.fpRegradingResultDocs.Attach(model);
            //DbContext.Entry(model).Property(x => x.IsReturnedToPurchasing).IsModified = true;
            return DbContext.SaveChanges();
        }

        public Tuple<List<FpRegradingResultDocsReportViewModel>, int> GetReport(string unitName, string code, string productName, bool? isReturn, bool? isReturnedToPurchasing, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order)
        {
            IQueryable<Models.FpRegradingResultDocs> Query = this.DbSet;

            DateTimeOffset dateFromFilter = (dateFrom == null ? new DateTime(1970, 1, 1) : dateFrom.Value.Date);
            DateTimeOffset dateToFilter = (dateTo == null ? DateTimeOffset.UtcNow.Date : dateTo.Value.Date);

            Query = Query
                .Where(p => p._IsDeleted == false &&
                    p.UnitName == (string.IsNullOrWhiteSpace(unitName) ? p.UnitName : unitName) &&
                    p.Code == (string.IsNullOrWhiteSpace(code) ? p.Code : code) &&
                    p.ProductName == (string.IsNullOrWhiteSpace(productName) ? p.ProductName : productName) &&
                    p.IsReturn == (isReturn == null ? p.IsReturn : isReturn) &&
                    p.IsReturnedToPurchasing == (isReturnedToPurchasing == null ? p.IsReturnedToPurchasing : isReturnedToPurchasing) &&
                    p.Date.Date >= dateFromFilter &&
                    p.Date.Date <= dateToFilter
                );

            Query = Query
               .Select(s => new Models.FpRegradingResultDocs
               {
                   Id = s.Id,
                   Code = s.Code,
                   UnitName = s.UnitName,
                   ProductName = s.ProductName,
                   SupplierName = s.SupplierName,
                   _CreatedUtc = s._CreatedUtc,
                   _LastModifiedUtc = s._LastModifiedUtc,
                   IsReturn = s.IsReturn,
                   IsReturnedToPurchasing = s.IsReturnedToPurchasing,
                   Details = s.Details.Select(p => new FpRegradingResultDocsDetails { FpReturProInvDocsId = p.FpReturProInvDocsId, Length = p.Length }).Where(i => i.FpReturProInvDocsId.Equals(s.Id)).ToList()
               });

            #region Order

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

            #endregion Order

            #region Paging

            Pageable<Models.FpRegradingResultDocs> pageable = new Pageable<Models.FpRegradingResultDocs>(Query, page - 1, size);
            List<Models.FpRegradingResultDocs> Data = pageable.Data.ToList<Models.FpRegradingResultDocs>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            List<FpRegradingResultDocsReportViewModel> list = Data
                    .GroupBy(d => new { d.Id, d.Code, d.UnitName, d.ProductName, d.SupplierName, d._CreatedUtc, d.IsReturn, d.IsReturnedToPurchasing })
                    .Select(s => new FpRegradingResultDocsReportViewModel
                    {
                        Code = s.First().Code,
                        ProductName = s.First().ProductName,
                        UnitName = s.First().UnitName,
                        _CreatedUtc = s.First()._CreatedUtc,
                        TotalQuantity = s.Sum(d => d.Details.Count()),
                        TotalLength = s.Sum(d => d.Details.Sum(p => p.Length)),
                        IsReturn = s.First().IsReturn,
                        IsReturnedToPurchasing = s.First().IsReturnedToPurchasing,
                        SupplierName = s.First().SupplierName
                    }).ToList();

            return Tuple.Create(list, TotalData);
        }
    }
}
