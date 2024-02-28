using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentReceiptSubconWasteProduction.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentReceiptSubconWasteProductionViewModel.ReceiptWaste;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentReceiptSubconWasteProduction
{
    public class GarmentSubconReceiptWasteProductionService : IGarmentSubconReceiptWasteProductionService
    {
        private const string UserAgent = "GarmentReceiptWasteProductionService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentSubconReceiptWasteProductions> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;
        private readonly ILogHistoryService logHistoryService;
        public GarmentSubconReceiptWasteProductionService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentSubconReceiptWasteProductions>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
        }

        public GarmentSubconReceiptWasteProductions MapToModel(GarmentSubconReceiptWasteProductionViewModel viewModel)
        {
            GarmentSubconReceiptWasteProductions model = new GarmentSubconReceiptWasteProductions();
            PropertyCopier<GarmentSubconReceiptWasteProductionViewModel, GarmentSubconReceiptWasteProductions>.Copy(viewModel, model);

            model.Items = new List<GarmentSubconReceiptWasteProductionItems>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentSubconReceiptWasteProductionItems modelItem = new GarmentSubconReceiptWasteProductionItems();
                PropertyCopier<GarmentSubconReceiptWasteProductionItemViewModel, GarmentSubconReceiptWasteProductionItems>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Product != null)
                {
                    modelItem.ProductId = long.Parse(viewModelItem.Product.Id);
                    modelItem.ProductCode = viewModelItem.Product.Code;
                    modelItem.ProductName = viewModelItem.Product.Name;
                }
                model.Items.Add(modelItem);
            }
            return model;
        }

        public GarmentSubconReceiptWasteProductionViewModel MapToViewModel(GarmentSubconReceiptWasteProductions model)
        {
            GarmentSubconReceiptWasteProductionViewModel viewModel = new GarmentSubconReceiptWasteProductionViewModel();
            PropertyCopier<GarmentSubconReceiptWasteProductions, GarmentSubconReceiptWasteProductionViewModel>.Copy(model, viewModel);
            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentSubconReceiptWasteProductionItemViewModel>();

                foreach (var modelItem in model.Items)
                {
                    GarmentSubconReceiptWasteProductionItemViewModel viewModelItem = new GarmentSubconReceiptWasteProductionItemViewModel();
                    PropertyCopier<GarmentSubconReceiptWasteProductionItems, GarmentSubconReceiptWasteProductionItemViewModel>.Copy(modelItem, viewModelItem);

                    viewModelItem.Product = new ProductViewModel
                    {
                        Id = modelItem.ProductId.ToString(),
                        Code = modelItem.ProductCode,
                        Name = modelItem.ProductName
                    };

                    viewModel.Items.Add(viewModelItem);
                }
            }
            return viewModel;
        }

         public ReadResponse<GarmentSubconReceiptWasteProductions> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentSubconReceiptWasteProductions> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "GarmentReceiptWasteNo", "WasteType"
            };
            Query = QueryHelper<GarmentSubconReceiptWasteProductions>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentSubconReceiptWasteProductions>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentSubconReceiptWasteProductions>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "GarmentReceiptWasteNo", "SourceName", "ReceiptDate","WasteType","TotalAval","DestinationName"
            };

            Query = Query.Select(s => new GarmentSubconReceiptWasteProductions
            {
                Id = s.Id,
                GarmentReceiptWasteNo = s.GarmentReceiptWasteNo,
                WasteType = s.WasteType,
                SourceName = s.SourceName,
                DestinationName = s.DestinationName,
                TotalAval=s.TotalAval,
                ReceiptDate = s.ReceiptDate
            });

            Pageable<GarmentSubconReceiptWasteProductions> pageable = new Pageable<GarmentSubconReceiptWasteProductions>(Query, page - 1, size);
            List<GarmentSubconReceiptWasteProductions> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentSubconReceiptWasteProductions>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentSubconReceiptWasteProductions> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(GarmentSubconReceiptWasteProductions model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.GarmentReceiptWasteNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }

                    DbSet.Add(model);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Create Penerimaan Aval Sampah Sapuan Dan TC Kecil - " + model.GarmentReceiptWasteNo);

                    Created = await DbContext.SaveChangesAsync();
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

        public async Task<int> UpdateAsync(int id, GarmentSubconReceiptWasteProductions model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentSubconReceiptWasteProductions existingModel = await DbSet.Include(x => x.Items).Where(w => w.Id == id).FirstOrDefaultAsync();

                    if (existingModel.ReceiptDate != model.ReceiptDate)
                    {
                        existingModel.ReceiptDate = model.ReceiptDate;
                    }
                    if (existingModel.Remark != model.Remark)
                    {
                        existingModel.Remark = model.Remark;
                    }
                    if (existingModel.TotalAval != model.TotalAval)
                    {
                        existingModel.TotalAval = model.TotalAval;
                    }

                    //GarmentSubconReceiptWasteProductionItems existingItem = await DbSet.i
                    foreach (var item in existingModel.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    foreach (var item in model.Items)
                    {
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        item.FlagForCreate(IdentityService.Username, UserAgent);

                        existingModel.Items.Add(item);
                    }

                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Update Penerimaan Aval Sampah Sapuan Dan TC Kecil - " + model.GarmentReceiptWasteNo);

                    Updated = await DbContext.SaveChangesAsync();

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

        public async Task<int> DeleteAsync(int id)
        {
            int Deleted = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentSubconReceiptWasteProductions model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Delete Penerimaan Aval Sampah Sapuan Dan TC Kecil - " + model.GarmentReceiptWasteNo);

                    Deleted = await DbContext.SaveChangesAsync();
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

        private string GenerateNo(GarmentSubconReceiptWasteProductions model)
        {
            string no  = "ATSM" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM") + model._CreatedUtc.ToString("dd") + DateTime.Now.ToString("ffff");
            

            return no;
        }



    }
}
