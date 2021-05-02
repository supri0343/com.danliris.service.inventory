using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryDocumentViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices
{
    public class MaterialsRequestNoteService : BasicService<InventoryDbContext, MaterialsRequestNote>, IMap<MaterialsRequestNote, MaterialsRequestNoteViewModel>
    {
        public MaterialsRequestNoteService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<MaterialsRequestNote>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MaterialsRequestNote> Query = this.DbContext.MaterialsRequestNotes;

            List<string> SearchAttributes = new List<string>()
                {
                    "UnitName", "RequestType", "Code", "MaterialsRequestNote_Items.ProductionOrderNo"
                };
            Query = ConfigureSearch(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
                {
                    "Id", "Code", "Unit", "RequestType", "Remark", "MaterialsRequestNote_Items", "_LastModifiedUtc", "IsCompleted"
                };
            Query = Query
                .Select(mrn => new MaterialsRequestNote
                {
                    Id = mrn.Id,
                    Code = mrn.Code,
                    UnitId = mrn.UnitId,
                    UnitCode = mrn.UnitCode,
                    UnitName = mrn.UnitName,
                    IsDistributed = mrn.IsDistributed,
                    IsCompleted = mrn.IsCompleted,
                    RequestType = mrn.RequestType,
                    _LastModifiedUtc = mrn._LastModifiedUtc,
                    MaterialsRequestNote_Items = mrn.MaterialsRequestNote_Items.Select(p => new MaterialsRequestNote_Item { MaterialsRequestNoteId = p.MaterialsRequestNoteId, ProductionOrderNo = p.ProductionOrderNo }).Where(i => i.MaterialsRequestNoteId.Equals(mrn.Id)).ToList()
                });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = ConfigureOrder(Query, OrderDictionary);

            Pageable<MaterialsRequestNote> pageable = new Pageable<MaterialsRequestNote>(Query, Page - 1, Size);
            List<MaterialsRequestNote> Data = pageable.Data.ToList<MaterialsRequestNote>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<MaterialsRequestNote> CustomCodeGenerator(MaterialsRequestNote Model)
        {
            Model.Type = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "P" : "F";
            var lastData = await this.DbSet.Where(w => w.UnitCode == Model.UnitCode).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");
            string Month = Now.ToString("MM");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.Code = $"SPB{Model.Type}{Month}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"SPB{Model.Type}{Month}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"SPB{Model.Type}{Month}{Year}{Number}";
                }
            }

            return Model;
        }

        public void UpdateIsRequestedProductionOrder(List<string> productionOrderIds, string context)
        {
            string productionOrderUri;
            if(context == "DELETE")
            {
                productionOrderUri = "sales/production-orders/update-requested-false";
            }
            else
            {
                productionOrderUri = "sales/production-orders/update-requested-true";
            }
           

            var data = new
            {
                context = context,
                ids = productionOrderIds
            };

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = httpClient.PutAsync($"{APIEndpoint.Sales}{productionOrderUri}", new StringContent(JsonConvert.SerializeObject(productionOrderIds).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public void UpdateIsCompletedProductionOrder(string productionOrderId)
        {
            string productionOrderUri = "sales/production-orders/update-iscompleted-true";

            //var data = new
            //{
            //    contextAndIds = contextAndIds
            //};

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = httpClient.PutAsync($"{APIEndpoint.Sales}{productionOrderUri}", new StringContent(productionOrderId, Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public void UpdateDistributedQuantityProductionOrder(List<SppParams> contextAndIds)
        {
            string productionOrderUri = "sales/production-orders/update-distributed-quantity";

            var data = new
            {
                data = contextAndIds
            };

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = httpClient.PutAsync($"{APIEndpoint.Sales}{productionOrderUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        //public void UpdateInventorySummary(List<InventorySummaryViewModel> item)
        //{
        //    string inventorySummary = "inventory/inventory-summary/update/all-summary";

        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        //    var response = httpClient.PutAsync($"{APIEndpoint.Inventory}{inventorySummary}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, General.JsonMediaType)).Result;
        //    response.EnsureSuccessStatusCode();
        //}

        public override async Task<int> CreateModel(MaterialsRequestNote Model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> productionOrderIds = new List<string>();
                    Model = await this.CustomCodeGenerator(Model);
                    Created = await this.CreateAsync(Model);


                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        productionOrderIds.Add(item.ProductionOrderId);
                    }

                    //if (Model.RequestType != "PEMBELIAN")
                    //{
                    //    await this.CreateInventoryDocument(Model, "OUT");
                    //}

                    UpdateIsRequestedProductionOrder(productionOrderIds, "CREATE");
                    transaction.Commit();
                }
                catch (ServiceValidationExeption e)
                {
                    throw new ServiceValidationExeption(e.ValidationContext, e.ValidationResults);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return Created;
        }

        public override async Task<MaterialsRequestNote> ReadModelById(int id)
        {
            return await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.MaterialsRequestNote_Items)
                .FirstOrDefaultAsync();
        }

        public class SppParams
        {
            public string context { get; set; }
            public string id { get; set; }
            public double distributedQuantity { get; set; }
        }

        public async Task UpdateIsCompleted(int Id, MaterialsRequestNote Model)
        {
            {
                try
                {
                    int CountIsIncomplete = 0;
                    List<SppParams> contextAndIds = new List<SppParams>();
                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        SppParams sppParams = new SppParams();
                        if (!item.ProductionOrderIsCompleted)
                        {
                            CountIsIncomplete += 1;
                            sppParams.context = "INCOMPLETE";
                            sppParams.id = item.ProductionOrderId;
                        }
                        else
                        {
                            sppParams.context = "COMPLETE";
                            sppParams.id = item.ProductionOrderId;
                        }

                        contextAndIds.Add(sppParams);
                        UpdateIsCompletedProductionOrder(item.ProductionOrderId);
                    }
                    

                    if (CountIsIncomplete == 0)
                    {
                        Model.IsCompleted = true;
                    }
                    else
                    {
                        Model.IsCompleted = false;
                    }

                    await UpdateModel(Id, Model);


                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }

        }

        public void UpdateDistributedQuantity(int Id, MaterialsRequestNote Model)
        {
            {
                try
                {
                    List<SppParams> contextQuantityAndIds = new List<SppParams>();
                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        SppParams sppParams = new SppParams
                        {
                            id = item.ProductionOrderId,
                            distributedQuantity = item.DistributedLength
                        };

                        contextQuantityAndIds.Add(sppParams);
                    }
                    UpdateDistributedQuantityProductionOrder(contextQuantityAndIds);
                }
                catch (Exception)
                {
                }
            }
        }

        public override async Task<int> UpdateModel(int Id, MaterialsRequestNote Model)
        {

            MaterialsRequestNote_ItemService materialsRequestNote_ItemService = this.ServiceProvider.GetService<MaterialsRequestNote_ItemService>();
            materialsRequestNote_ItemService.Username = this.Username;

            int Updated = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {

                    var dbModel = await ReadModelById(Id);

                    dbModel.Remark = Model.Remark;
                    Updated = await UpdateAsync(dbModel.Id, dbModel);
                    var deletedDetails = dbModel.MaterialsRequestNote_Items.Where(x => !Model.MaterialsRequestNote_Items.Any(y => x.Id == y.Id));
                    var updatedDetails = dbModel.MaterialsRequestNote_Items.Where(x => Model.MaterialsRequestNote_Items.Any(y => x.Id == y.Id));
                    var addedDetails = Model.MaterialsRequestNote_Items.Where(x => !dbModel.MaterialsRequestNote_Items.Any(y => y.Id == x.Id));
                    List<string> deletedProductionOrderIds = new List<string>();
                    List<string> newProductionOrderIds = new List<string>();
                    foreach (var item in deletedDetails)
                    {
                        Updated += await materialsRequestNote_ItemService.DeleteModel(item.Id);
                        deletedProductionOrderIds.Add(item.ProductionOrderId);
                    }

                    foreach(var item in updatedDetails)
                    {
                        var selectedDetail = Model.MaterialsRequestNote_Items.FirstOrDefault(x => x.Id == item.Id);
                        
                        if (item.ProductionOrderId != selectedDetail.ProductionOrderId)
                        {
                            newProductionOrderIds.Add(selectedDetail.ProductionOrderId);
                            deletedProductionOrderIds.Add(item.ProductionOrderId);
                        }

                        item.ProductionOrderId = selectedDetail.ProductionOrderId;
                        item.ProductionOrderIsCompleted = selectedDetail.ProductionOrderIsCompleted;
                        item.ProductionOrderNo = selectedDetail.ProductionOrderNo;
                        item.ProductId = selectedDetail.ProductId;
                        item.ProductCode = selectedDetail.ProductCode;
                        item.ProductName = selectedDetail.ProductName;
                        item.Grade = selectedDetail.Grade;
                        item.Length = selectedDetail.Length;
                        item.Remark = selectedDetail.Remark;
                        item.DistributedLength = selectedDetail.DistributedLength;
                        item.OrderQuantity = selectedDetail.OrderQuantity;
                        item.OrderTypeCode = selectedDetail.OrderTypeCode;
                        item.OrderTypeId = selectedDetail.OrderTypeId;
                        item.OrderTypeName = selectedDetail.OrderTypeName;

                        Updated += await materialsRequestNote_ItemService.UpdateModel(item.Id, item);

                    }

                    foreach(var item in addedDetails)
                    {
                        item.MaterialsRequestNoteId = Id;
                        Updated += await materialsRequestNote_ItemService.CreateModel(item);
                        newProductionOrderIds.Add(item.ProductionOrderId);
                    }
                    UpdateIsRequestedProductionOrder(deletedProductionOrderIds, "DELETE");
                    UpdateIsRequestedProductionOrder(newProductionOrderIds, "CREATE");

                    //HashSet<object> materialsRequestNote_Items = new HashSet<object>(materialsRequestNote_ItemService.DbSet
                    //    .Where(w => w.MaterialsRequestNoteId.Equals(Id))
                    //    .AsNoTracking());
                    ////.Select(s => new { Id = s.Id, ProductionOrderId = s.ProductionOrderId }));

                    //Updated = await this.UpdateAsync(Id, Model);


                    //List<MaterialsRequestNote_Item> itemForUpdateInventory = new List<MaterialsRequestNote_Item>();

                    //foreach (dynamic materialsRequestNote_Item in materialsRequestNote_Items)
                    //{
                    //    MaterialsRequestNote_Item model = Model.MaterialsRequestNote_Items.FirstOrDefault<dynamic>(prop => prop.Id.Equals(materialsRequestNote_Item.Id));
                    //    if (model == null)
                    //    {
                    //        await materialsRequestNote_ItemService.DeleteModel(materialsRequestNote_Item.Id);
                    //        productionOrderIds.Add(materialsRequestNote_Item.ProductionOrderId);
                    //        itemForUpdateInventory.Add(materialsRequestNote_Item);
                    //    }
                    //    else
                    //    {
                    //        double length = materialsRequestNote_Item.Length - model.Length;
                    //        materialsRequestNote_Item.Length = length;
                    //        itemForUpdateInventory.Add(materialsRequestNote_Item);
                    //        await materialsRequestNote_ItemService.UpdateModel(materialsRequestNote_Item.Id, model);
                    //    }
                    //}

                    //foreach (MaterialsRequestNote_Item materialsRequestNote_Item in Model.MaterialsRequestNote_Items)
                    //{
                    //    if (materialsRequestNote_Item.Id.Equals(0))
                    //    {
                    //        await materialsRequestNote_ItemService.CreateModel(materialsRequestNote_Item);
                    //        newProductionOrderIds.Add(materialsRequestNote_Item.ProductionOrderId);
                    //        double length = materialsRequestNote_Item.Length * -1;
                    //        materialsRequestNote_Item.Length = length;
                    //        itemForUpdateInventory.Add(materialsRequestNote_Item);
                    //    }
                    //}

                    ////if (Model.RequestType != "PEMBELIAN")
                    ////{
                    ////    Model.MaterialsRequestNote_Items = itemForUpdateInventory;
                    ////    await this.CreateInventoryDocument(Model, "IN");
                    ////}

                    //UpdateIsRequestedProductionOrder(productionOrderIds, "UPDATE");
                    //UpdateIsRequestedProductionOrder(newProductionOrderIds, "CREATE");
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Updated;
        }

        public override async Task<int> DeleteModel(int Id)
        {
            MaterialsRequestNote_ItemService materialsRequestNote_ItemService = this.ServiceProvider.GetService<MaterialsRequestNote_ItemService>();

            int Deleted = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    MaterialsRequestNote Model = await this.ReadModelById(Id);
                    Deleted = await this.DeleteAsync(Id);

                    
                    HashSet<object> materialsRequestNote_Items = new HashSet<object>(materialsRequestNote_ItemService.DbSet
                    .Where(w => w.MaterialsRequestNoteId.Equals(Id))
                    .AsNoTracking());

                    materialsRequestNote_ItemService.Username = this.Username;

                    foreach (dynamic materialsRequestNote_Item in materialsRequestNote_Items)
                    {
                        await materialsRequestNote_ItemService.DeleteModel(materialsRequestNote_Item.Id);
                    }

                    List<string> productionOrderIds = new List<string>();

                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        productionOrderIds.Add(item.ProductionOrderId);

                    }

                    //if (Model.RequestType != "PEMBELIAN")
                    //{
                    //    await this.CreateInventoryDocument(Model, "IN");
                    //}


                    UpdateIsRequestedProductionOrder(productionOrderIds, "DELETE");
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            return Deleted;
        }

        public override void OnCreating(MaterialsRequestNote model)
        {
            if (model.MaterialsRequestNote_Items.Count > 0)
            {
                MaterialsRequestNote_ItemService materialsRequestNote_ItemService = this.ServiceProvider.GetService<MaterialsRequestNote_ItemService>();

                materialsRequestNote_ItemService.Username = this.Username;
                foreach (MaterialsRequestNote_Item materialsRequestNoteItem in model.MaterialsRequestNote_Items)
                {
                    materialsRequestNote_ItemService.OnCreating(materialsRequestNoteItem);
                }
            }

            base.OnCreating(model);
            model._CreatedAgent = "Service";
            model._CreatedBy = this.Username;
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnUpdating(int id, MaterialsRequestNote model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(MaterialsRequestNote model)
        {
            base.OnDeleting(model);
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }

        public MaterialsRequestNoteViewModel MapToViewModel(MaterialsRequestNote model)
        {
            MaterialsRequestNoteViewModel viewModel = new MaterialsRequestNoteViewModel();

            PropertyCopier<MaterialsRequestNote, MaterialsRequestNoteViewModel>.Copy(model, viewModel);

            UnitViewModel Unit = new UnitViewModel()
            {
                Id = model.UnitId,
                Code = model.UnitCode,
                Name = model.UnitName
            };

            viewModel.Code = model.Code;
            viewModel.Unit = Unit;
            viewModel.RequestType = model.RequestType;
            viewModel.Remark = model.Remark;

            viewModel.MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>();
            if (model.MaterialsRequestNote_Items != null)
            {
                foreach (MaterialsRequestNote_Item materialsRequestNote_Item in model.MaterialsRequestNote_Items)
                {
                    MaterialsRequestNote_ItemViewModel materialsRequestNote_ItemViewModel = new MaterialsRequestNote_ItemViewModel();
                    PropertyCopier<MaterialsRequestNote_Item, MaterialsRequestNote_ItemViewModel>.Copy(materialsRequestNote_Item, materialsRequestNote_ItemViewModel);

                    OrderTypeViewModel OrderType = new OrderTypeViewModel()
                    {
                        Id = materialsRequestNote_Item.OrderTypeId,
                        Code = materialsRequestNote_Item.OrderTypeCode,
                        Name = materialsRequestNote_Item.OrderTypeName
                    };

                    ProductionOrderViewModel ProductionOrder = new ProductionOrderViewModel()
                    {
                        Id = materialsRequestNote_Item.ProductionOrderId,
                        OrderNo = materialsRequestNote_Item.ProductionOrderNo,
                        OrderQuantity = materialsRequestNote_Item.OrderQuantity,
                        IsCompleted = materialsRequestNote_Item.ProductionOrderIsCompleted,
                        DistributedQuantity = materialsRequestNote_Item.DistributedLength,
                        OrderType = OrderType
                    };
                    materialsRequestNote_ItemViewModel.ProductionOrder = ProductionOrder;

                    ProductViewModel Product = new ProductViewModel()
                    {
                        Id = materialsRequestNote_Item.ProductId,
                        Code = materialsRequestNote_Item.ProductCode,
                        Name = materialsRequestNote_Item.ProductName
                    };
                    materialsRequestNote_ItemViewModel.Product = Product;

                    materialsRequestNote_ItemViewModel.Length = materialsRequestNote_Item.Length;
                    materialsRequestNote_ItemViewModel.DistributedLength = materialsRequestNote_Item.DistributedLength;

                    viewModel.MaterialsRequestNote_Items.Add(materialsRequestNote_ItemViewModel);
                }
            }

            return viewModel;
        }

        public MaterialsRequestNote MapToModel(MaterialsRequestNoteViewModel viewModel)
        {
            MaterialsRequestNote model = new MaterialsRequestNote();

            PropertyCopier<MaterialsRequestNoteViewModel, MaterialsRequestNote>.Copy(viewModel, model);

            model.UnitId = viewModel.Unit.Id;
            model.UnitCode = viewModel.Unit.Code;
            model.UnitName = viewModel.Unit.Name;
            model.RequestType = viewModel.RequestType;
            model.Remark = viewModel.Remark;

            model.MaterialsRequestNote_Items = new List<MaterialsRequestNote_Item>();

            foreach (MaterialsRequestNote_ItemViewModel materialsRequestNote_ItemViewModel in viewModel.MaterialsRequestNote_Items)
            {
                MaterialsRequestNote_Item materialsRequestNote_Item = new MaterialsRequestNote_Item();

                PropertyCopier<MaterialsRequestNote_ItemViewModel, MaterialsRequestNote_Item>.Copy(materialsRequestNote_ItemViewModel, materialsRequestNote_Item);

                if (!viewModel.RequestType.Equals("PEMBELIAN") && !viewModel.RequestType.Equals("TEST"))
                {

                    materialsRequestNote_Item.ProductionOrderId = materialsRequestNote_ItemViewModel.ProductionOrder.Id;
                    materialsRequestNote_Item.ProductionOrderNo = materialsRequestNote_ItemViewModel.ProductionOrder.OrderNo;
                    materialsRequestNote_Item.ProductionOrderIsCompleted = materialsRequestNote_ItemViewModel.ProductionOrder.IsCompleted;
                    materialsRequestNote_Item.OrderQuantity = (double)materialsRequestNote_ItemViewModel.ProductionOrder.OrderQuantity;
                    materialsRequestNote_Item.OrderTypeId = materialsRequestNote_ItemViewModel.ProductionOrder.OrderType.Id;
                    materialsRequestNote_Item.OrderTypeCode = materialsRequestNote_ItemViewModel.ProductionOrder.OrderType.Code;
                    materialsRequestNote_Item.OrderTypeName = materialsRequestNote_ItemViewModel.ProductionOrder.OrderType.Name;
                }

                materialsRequestNote_Item.ProductId = materialsRequestNote_ItemViewModel.Product.Id;
                materialsRequestNote_Item.ProductCode = materialsRequestNote_ItemViewModel.Product.Code;
                materialsRequestNote_Item.ProductName = materialsRequestNote_ItemViewModel.Product.Name;
                materialsRequestNote_Item.Length = materialsRequestNote_ItemViewModel.Length != null ? (double)materialsRequestNote_ItemViewModel.Length : 0;
                materialsRequestNote_Item.DistributedLength = materialsRequestNote_ItemViewModel.DistributedLength != null ? (double)materialsRequestNote_ItemViewModel.DistributedLength : 0;

                model.MaterialsRequestNote_Items.Add(materialsRequestNote_Item);
            }

            return model;
        }

        public IQueryable<MaterialsRequestNoteReportViewModel> GetReportQuery(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            bool IsCompleted = !string.IsNullOrWhiteSpace(status) && status.ToUpper().Equals("SUDAH COMPLETE") ? true : false;
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in DbContext.MaterialsRequestNotes
                         join b in DbContext.MaterialsRequestNote_Items on a.Id equals b.MaterialsRequestNoteId
                         where a._IsDeleted == false
                             && a.Code == (string.IsNullOrWhiteSpace(materialsRequestNoteCode) ? a.Code : materialsRequestNoteCode)
                             && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                             && b.ProductionOrderId == (string.IsNullOrWhiteSpace(productionOrderId) ? b.ProductionOrderId : productionOrderId)
                             && b.ProductId == (string.IsNullOrWhiteSpace(productId) ? b.ProductId : productId)
                             && b.ProductionOrderIsCompleted == (string.IsNullOrWhiteSpace(status) ? b.ProductionOrderIsCompleted : IsCompleted)
                             && a._CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                             && a._CreatedUtc.AddHours(offset).Date <= DateTo.Date
                         select new MaterialsRequestNoteReportViewModel
                         {
                             Code = a.Code,
                             CreatedDate = a._CreatedUtc,
                             OrderNo = b.ProductionOrderNo,
                             ProductName = b.ProductName,
                             Grade = b.Grade,
                             OrderQuantity = b.OrderQuantity,
                             Length = b.Length,
                             DistributedLength = b.DistributedLength,
                             Status = b.ProductionOrderIsCompleted,
                             Remark = a.Remark,
                         });

            return Query;
        }

        public Tuple<List<MaterialsRequestNoteReportViewModel>, int> GetReport(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(materialsRequestNoteCode, productionOrderId, unitId, productId, status, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.CreatedDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<MaterialsRequestNoteReportViewModel> pageable = new Pageable<MaterialsRequestNoteReportViewModel>(Query, page - 1, size);
            List<MaterialsRequestNoteReportViewModel> Data = pageable.Data.ToList<MaterialsRequestNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public async Task CreateInventoryDocument(MaterialsRequestNote Model, string Type)
        {
            //string inventoryDocumentURI = "inventory/inventory-documents";
            string storageURI = "master/storages";
            string uomURI = "master/uoms";

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

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

            List<MaterialsRequestNote_Item> list = Model.MaterialsRequestNote_Items
            .GroupBy(m => new { m.ProductId, m.ProductCode, m.ProductName })
            .Select(s => new MaterialsRequestNote_Item
            {
                ProductId = s.First().ProductId,
                ProductCode = s.First().ProductCode,
                ProductName = s.First().ProductName,
                Length = s.Sum(d => d.Length)
            }).ToList();


            foreach (MaterialsRequestNote_Item item in list)
            {
                InventoryDocumentItem inventoryDocumentItem = new InventoryDocumentItem();
                inventoryDocumentItem.ProductId = int.Parse(item.ProductId);
                inventoryDocumentItem.ProductCode = item.ProductCode;
                inventoryDocumentItem.ProductName = item.ProductName;
                inventoryDocumentItem.Quantity = item.Length;
                inventoryDocumentItem.UomId = int.Parse(uom["Id"].ToString());
                inventoryDocumentItem.UomUnit = uom["Unit"].ToString();
                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            InventoryDocument inventoryDocument = new InventoryDocument
            {
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Model.Code,
                ReferenceType = "Surat Permintaan Barang",
                Type = Type,
                StorageId = int.Parse(storage["_id"].ToString()),
                StorageCode = storage["code"].ToString(),
                StorageName = storage["name"].ToString(),
                Items = inventoryDocumentItems
            };

            InventoryDocumentFacade inventoryDocumentFacade = (InventoryDocumentFacade)ServiceProvider.GetService(typeof(InventoryDocumentFacade));
            await inventoryDocumentFacade.Create(inventoryDocument,Username);
            //var response = httpClient.PostAsync($"{APIEndpoint.Inventory}{inventoryDocumentURI}", new StringContent(JsonConvert.SerializeObject(inventoryDocument).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            //response.EnsureSuccessStatusCode();
        }
    }
}
