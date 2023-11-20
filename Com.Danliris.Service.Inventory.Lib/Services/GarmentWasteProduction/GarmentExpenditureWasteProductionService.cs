using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
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
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ExpenditureWaste;
using Com.Danliris.Service.Inventory.Lib.Services.LogHistories;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction
{
    public class GarmentExpenditureWasteProductionService : IGarmentExpenditureWasteProductionService
    {
        private const string UserAgent = "GarmentExpenditureWasteProductionService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentExpenditureWasteProductions> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;
        private readonly ILogHistoryService logHistoryService;
        public GarmentExpenditureWasteProductionService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentExpenditureWasteProductions>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
            logHistoryService = (ILogHistoryService)serviceProvider.GetService(typeof(ILogHistoryService));
        }

        public GarmentExpenditureWasteProductions MapToModel(GarmentExpenditureWasteProductionViewModel viewModel)
        {
            GarmentExpenditureWasteProductions model = new GarmentExpenditureWasteProductions();
            PropertyCopier<GarmentExpenditureWasteProductionViewModel, GarmentExpenditureWasteProductions>.Copy(viewModel, model);

            model.Items = new List<GarmentExpenditureWasteProductionItems>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentExpenditureWasteProductionItems modelItem = new GarmentExpenditureWasteProductionItems();
                PropertyCopier<GarmentExpenditureWasteProductionItemViewModel, GarmentExpenditureWasteProductionItems>.Copy(viewModelItem, modelItem);
                
                model.Items.Add(modelItem);
            }
            return model;
        }

        public GarmentExpenditureWasteProductionViewModel MapToViewModel(GarmentExpenditureWasteProductions model)
        {
            GarmentExpenditureWasteProductionViewModel viewModel = new GarmentExpenditureWasteProductionViewModel();
            PropertyCopier<GarmentExpenditureWasteProductions, GarmentExpenditureWasteProductionViewModel>.Copy(model, viewModel);
            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentExpenditureWasteProductionItemViewModel>();

                foreach (var modelItem in model.Items)
                {
                    GarmentExpenditureWasteProductionItemViewModel viewModelItem = new GarmentExpenditureWasteProductionItemViewModel();
                    PropertyCopier<GarmentExpenditureWasteProductionItems, GarmentExpenditureWasteProductionItemViewModel>.Copy(modelItem, viewModelItem);

                    viewModel.Items.Add(viewModelItem);
                }
            }
            return viewModel;
        }

        public ReadResponse<GarmentExpenditureWasteProductions> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentExpenditureWasteProductions> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "GarmentExpenditureWasteNo", "WasteType","BCOutNo"
            };
            Query = QueryHelper<GarmentExpenditureWasteProductions>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentExpenditureWasteProductions>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentExpenditureWasteProductions>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "GarmentExpenditureWasteNo", "WasteType", "BCOutNo","ExpenditureDate","BCOutDate","ActualQuantity","ExpenditureTo","Description"
            };

            Query = Query.Select(s => new GarmentExpenditureWasteProductions
            {
                Id = s.Id,
                GarmentExpenditureWasteNo = s.GarmentExpenditureWasteNo,
                WasteType = s.WasteType,
                BCOutNo = s.BCOutNo,
                ExpenditureDate = s.ExpenditureDate,
                BCOutDate = s.BCOutDate,
                ActualQuantity = s.ActualQuantity,
                ExpenditureTo = s.ExpenditureTo,
                Description = s.Description
            });

            Pageable<GarmentExpenditureWasteProductions> pageable = new Pageable<GarmentExpenditureWasteProductions>(Query, page - 1, size);
            List<GarmentExpenditureWasteProductions> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentExpenditureWasteProductions>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentExpenditureWasteProductions> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(GarmentExpenditureWasteProductions model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.BCOutNo = "BC 25";
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.GarmentExpenditureWasteNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        GarmentReceiptWasteProductions receiptWaste = await DbContext.GarmentReceiptWasteProductions.Where(x => x.Id == item.GarmentReceiptWasteId).FirstOrDefaultAsync();

                        receiptWaste.IsUsed = true;
                        receiptWaste.FlagForUpdate(IdentityService.Username, UserAgent);
                        await DbContext.SaveChangesAsync();

                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }

                    DbSet.Add(model);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Create Pengeluaran Aval Sampah Sapuan Dan TC Kecil - " + model.GarmentExpenditureWasteNo);

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

        public async Task<int> UpdateAsync(int id, GarmentExpenditureWasteProductions model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentExpenditureWasteProductions existingModel = await DbSet.Include(x=> x.Items).Where(w => w.Id == id).FirstOrDefaultAsync();

                    if (existingModel.BCOutNo != model.BCOutNo)
                    {
                        existingModel.BCOutNo = model.BCOutNo;
                    }
                    if (existingModel.BCOutDate != model.BCOutDate)
                    {
                        existingModel.BCOutDate = model.BCOutDate;
                    }
                    if (existingModel.Description != model.Description)
                    {
                        existingModel.Description = model.Description;
                    }
                    if (existingModel.ExpenditureTo != model.ExpenditureTo)
                    {
                        existingModel.ExpenditureTo = model.ExpenditureTo;
                    }

              
       
                    foreach (var item in existingModel.Items)
                    {

                        GarmentReceiptWasteProductions receiptWaste = await DbContext.GarmentReceiptWasteProductions.Where(x => x.Id == item.GarmentReceiptWasteId).FirstOrDefaultAsync();
                        receiptWaste.FlagForUpdate(IdentityService.Username, UserAgent);
                        receiptWaste.IsUsed = false;

                        item.FlagForDelete(IdentityService.Username, UserAgent);

                        await DbContext.SaveChangesAsync();
                    }

                    foreach (var item in model.Items)
                    {
                     
                        GarmentReceiptWasteProductions receiptWaste = await DbContext.GarmentReceiptWasteProductions.Where(x => x.Id == item.GarmentReceiptWasteId).FirstOrDefaultAsync();
                        receiptWaste.FlagForUpdate(IdentityService.Username, UserAgent);
                        receiptWaste.IsUsed = true;
                        
                        GarmentExpenditureWasteProductionItems data = new GarmentExpenditureWasteProductionItems
                        {
                            GarmentExpenditureWasteId = item.GarmentExpenditureWasteId,
                            GarmentReceiptWasteNo = item.GarmentReceiptWasteNo,
                            GarmentReceiptWasteId = item.GarmentReceiptWasteId,
                            Quantity = item.Quantity,
                        };
                        data.FlagForUpdate(IdentityService.Username, UserAgent);
                        data.FlagForCreate(IdentityService.Username, UserAgent);

                        existingModel.Items.Add(data);

                        await DbContext.SaveChangesAsync();

                    }


                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Update Pengeluaran Aval Sampah Sapuan Dan TC Kecil - " + model.GarmentExpenditureWasteNo);

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
                    GarmentExpenditureWasteProductions model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        GarmentReceiptWasteProductions receiptWaste = await DbContext.GarmentReceiptWasteProductions.Where(x => x.Id == item.GarmentReceiptWasteId).FirstOrDefaultAsync();

                        receiptWaste.IsUsed = false;
                        receiptWaste.FlagForUpdate(IdentityService.Username, UserAgent);
                        await DbContext.SaveChangesAsync();

                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }
                    //Create Log History
                    logHistoryService.CreateAsync("GUDANG SISA", "Delete Pengeluaran Aval Sampah Sapuan Dan TC Kecil - " + model.GarmentExpenditureWasteNo);


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

        private string GenerateNo(GarmentExpenditureWasteProductions model)
        {
            string no = "";

            if (model.WasteType == "AVAL TC KECIL")
                no = "TCK" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM") + model._CreatedUtc.ToString("dd") + DateTime.Now.ToString("ffff");
            else
            {
                no = "SSK" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM") + model._CreatedUtc.ToString("dd") + DateTime.Now.ToString("ffff");
            }

            return no;
        }



    }
}
