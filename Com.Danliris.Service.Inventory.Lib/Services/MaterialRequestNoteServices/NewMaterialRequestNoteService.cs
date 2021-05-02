using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices
{
    public class NewMaterialRequestNoteService : IMaterialRequestNoteService
    {
        private const string UserAgent = "inventory-service";
        protected DbSet<MaterialsRequestNote> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public NewMaterialRequestNoteService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<MaterialsRequestNote>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public class SppParams
        {
            public string context { get; set; }
            public string id { get; set; }
            public double distributedQuantity { get; set; }
        }

        public async Task<int> CreateAsync(MaterialsRequestNote model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> productionOrderIds = new List<string>();
                    model = await this.CustomCodeGenerator(model);
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);
                    foreach (var item in model.MaterialsRequestNote_Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        productionOrderIds.Add(item.ProductionOrderId);
                    }
                    DbSet.Add(model);

                    Created = await DbContext.SaveChangesAsync();


                    UpdateIsRequestedProductionOrder(productionOrderIds, "CREATE");
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
            int Deleted = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> productionOrderIds = new List<string>();
                    MaterialsRequestNote Model = await ReadByIdAsync(id);
                    if (Model == null)
                        throw new Exception("data not found");
                    Model.FlagForDelete(IdentityService.Username, UserAgent);

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in Model.MaterialsRequestNote_Items)
                    {
                        await DeleteItem(item);
                        productionOrderIds.Add(item.ProductionOrderId);
                    }


                    UpdateIsRequestedProductionOrder(productionOrderIds, "DELETE");
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Deleted;
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
                    materialsRequestNote_Item.OrderQuantity = materialsRequestNote_ItemViewModel.ProductionOrder.OrderQuantity.GetValueOrDefault();
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

        public ReadResponse<MaterialsRequestNote> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<MaterialsRequestNote> Query = this.DbContext.MaterialsRequestNotes;

            List<string> SearchAttributes = new List<string>()
                {
                    "UnitName", "RequestType", "Code", "MaterialsRequestNote_Items.ProductionOrderNo"
                };
            Query = QueryHelper<MaterialsRequestNote>.Search(Query, SearchAttributes, keyword);

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

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<MaterialsRequestNote>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<MaterialsRequestNote>.Order(Query, OrderDictionary);

            Pageable<MaterialsRequestNote> pageable = new Pageable<MaterialsRequestNote>(Query, page - 1, size);
            List<MaterialsRequestNote> Data = pageable.Data.ToList<MaterialsRequestNote>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<MaterialsRequestNote>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<MaterialsRequestNote> ReadByIdAsync(int id)
        {
            return await this.DbSet
                .Include(d => d.MaterialsRequestNote_Items)
                .FirstOrDefaultAsync(d => d.Id.Equals(id) && d._IsDeleted.Equals(false));

        }

        public async Task<int> UpdateAsync(int id, MaterialsRequestNote model)
        {

            int Updated = 0;
            var internalTransaction = DbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? DbContext.Database.CurrentTransaction : DbContext.Database.BeginTransaction();

            try
            {

                var dbModel = await ReadByIdAsync(id);

                if (dbModel == null)
                    throw new Exception("data not found");


                dbModel.Remark = model.Remark;
                dbModel.IsCompleted = model.IsCompleted;
                dbModel.IsDistributed = model.IsDistributed;
                dbModel.RequestType = model.RequestType;
                dbModel.Type = model.Type;
                dbModel.UnitCode = model.UnitCode;
                dbModel.UnitId = model.UnitId;
                dbModel.UnitName = model.UnitName;
                dbModel.FlagForUpdate(IdentityService.Username, UserAgent);
                //DbSet.Update(dbModel);
                Updated = await DbContext.SaveChangesAsync();

                var deletedDetails = dbModel.MaterialsRequestNote_Items.Where(x => !model.MaterialsRequestNote_Items.Any(y => x.Id == y.Id));
                var updatedDetails = dbModel.MaterialsRequestNote_Items.Where(x => model.MaterialsRequestNote_Items.Any(y => x.Id == y.Id));
                var addedDetails = model.MaterialsRequestNote_Items.Where(x => !dbModel.MaterialsRequestNote_Items.Any(y => y.Id == x.Id));
                List<string> deletedProductionOrderIds = new List<string>();
                List<string> newProductionOrderIds = new List<string>();
                foreach (var item in deletedDetails)
                {
                    Updated += await DeleteItem(item);
                    deletedProductionOrderIds.Add(item.ProductionOrderId);
                }

                foreach (var item in updatedDetails)
                {
                    var selectedDetail = model.MaterialsRequestNote_Items.FirstOrDefault(x => x.Id == item.Id);

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

                    Updated += await UpdateItem(item);

                }

                foreach (var item in addedDetails)
                {
                    item.MaterialsRequestNoteId = id;
                    Updated += await CreateItem(item);
                    newProductionOrderIds.Add(item.ProductionOrderId);
                }
                UpdateIsRequestedProductionOrder(deletedProductionOrderIds, "DELETE");
                UpdateIsRequestedProductionOrder(newProductionOrderIds, "CREATE");


                if (internalTransaction)
                    transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }


            return Updated;
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
            if (context == "DELETE")
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

            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));

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

            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));

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

            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));
            var response = httpClient.PutAsync($"{APIEndpoint.Sales}{productionOrderUri}", new StringContent(JsonConvert.SerializeObject(contextAndIds).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateIsCompleted(int Id, MaterialsRequestNote Model)
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

                await UpdateAsync(Id, Model);


            }
            catch (Exception e)
            {
                throw e;
            };
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
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private async Task<int> DeleteItem(MaterialsRequestNote_Item item)
        {
            item.FlagForDelete(IdentityService.Username, UserAgent);
            return await DbContext.SaveChangesAsync();
        }


        private async Task<int> CreateItem(MaterialsRequestNote_Item item)
        {
            item.FlagForCreate(IdentityService.Username, UserAgent);
            item.FlagForUpdate(IdentityService.Username, UserAgent);
            DbContext.MaterialsRequestNote_Items.Add(item);
            return await DbContext.SaveChangesAsync();
        }

        private async Task<int> UpdateItem(MaterialsRequestNote_Item item)
        {
            item.FlagForUpdate(IdentityService.Username, UserAgent);
            return await DbContext.SaveChangesAsync();
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
        //public void UpdateInventorySummary(List<InventorySummaryViewModel> item)
        //{
        //    string inventorySummary = "inventory/inventory-summary/update/all-summary";

        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        //    var response = httpClient.PutAsync($"{APIEndpoint.Inventory}{inventorySummary}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, General.JsonMediaType)).Result;
        //    response.EnsureSuccessStatusCode();
        //}
    }
}
