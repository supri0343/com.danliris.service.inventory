using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction
{
    public class GarmentReceiptWasteProductionService : IGarmentReceiptWasteProductionService
    {
        private const string UserAgent = "GarmentReceiptWasteProductionService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentReceiptWasteProductions> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;
        private readonly ILogHistoryService logHistoryService;
        public GarmentReceiptWasteProductionService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentReceiptWasteProductions>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
        }

        public GarmentReceiptWasteProductions MapToModel(GarmentReceiptWasteProductionViewModel viewModel)
        {
            GarmentReceiptWasteProductions model = new GarmentReceiptWasteProductions();
            PropertyCopier<GarmentReceiptWasteProductionViewModel, GarmentReceiptWasteProductions>.Copy(viewModel, model);

            model.Items = new List<GarmentReceiptWasteProductionItems>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentReceiptWasteProductionItems modelItem = new GarmentReceiptWasteProductionItems();
                PropertyCopier<GarmentReceiptWasteProductionItemViewModel, GarmentReceiptWasteProductionItems>.Copy(viewModelItem, modelItem);

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

        public GarmentReceiptWasteProductionViewModel MapToViewModel(GarmentReceiptWasteProductions model)
        {
            GarmentReceiptWasteProductionViewModel viewModel = new GarmentReceiptWasteProductionViewModel();
            PropertyCopier<GarmentReceiptWasteProductions, GarmentReceiptWasteProductionViewModel>.Copy(model, viewModel);
            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentReceiptWasteProductionItemViewModel>();

                foreach (var modelItem in model.Items)
                {
                    GarmentReceiptWasteProductionItemViewModel viewModelItem = new GarmentReceiptWasteProductionItemViewModel();
                    PropertyCopier<GarmentReceiptWasteProductionItems, GarmentReceiptWasteProductionItemViewModel>.Copy(modelItem, viewModelItem);

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

         public ReadResponse<GarmentReceiptWasteProductions> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentReceiptWasteProductions> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "GarmentReceiptWasteNo", "WasteType"
            };
            Query = QueryHelper<GarmentReceiptWasteProductions>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentReceiptWasteProductions>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentReceiptWasteProductions>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "GarmentReceiptWasteNo", "SourceName", "ReceiptDate","WasteType","TotalAval","DestinationName"
            };

            Query = Query.Select(s => new GarmentReceiptWasteProductions
            {
                Id = s.Id,
                GarmentReceiptWasteNo = s.GarmentReceiptWasteNo,
                WasteType = s.WasteType,
                SourceName = s.SourceName,
                DestinationName = s.DestinationName,
                TotalAval=s.TotalAval,
                ReceiptDate = s.ReceiptDate
            });

            Pageable<GarmentReceiptWasteProductions> pageable = new Pageable<GarmentReceiptWasteProductions>(Query, page - 1, size);
            List<GarmentReceiptWasteProductions> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentReceiptWasteProductions>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentReceiptWasteProductions> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(GarmentReceiptWasteProductions model)
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

        public async Task<int> UpdateAsync(int id, GarmentReceiptWasteProductions model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentReceiptWasteProductions existingModel = await DbSet.Include(x => x.Items).Where(w => w.Id == id).FirstOrDefaultAsync();

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

                    //GarmentReceiptWasteProductionItems existingItem = await DbSet.i
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
                    GarmentReceiptWasteProductions model = await ReadByIdAsync(id);
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

        private string GenerateNo(GarmentReceiptWasteProductions model)
        {
            string no = "";

            if(model.WasteType == "AVAL TC KECIL")
                no = "TCM" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM") + model._CreatedUtc.ToString("dd") + DateTime.Now.ToString("ffff");
            else
            {
                no = "SSM" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM") + model._CreatedUtc.ToString("dd") + DateTime.Now.ToString("ffff");
            }

            return no;
        }



    }
}
