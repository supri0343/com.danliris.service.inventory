using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Lib.Services.FpReturnFromBuyers
{
    public class FpReturnFromBuyerService : IFpReturnFromBuyerService
    {
        private const string UserAgent = "Facade";
        private readonly InventoryDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IInventoryDocumentService _inventoryDocumentService;
        private readonly DbSet<FpReturnFromBuyerModel> _dbSet;
        private readonly DbSet<FpReturnFromBuyerDetailModel> _dbDetailSet;
        private readonly DbSet<FpReturnFromBuyerItemModel> _dbItemSet;

        public FpReturnFromBuyerService(InventoryDbContext dbContext, IServiceProvider serviceProvider, IInventoryDocumentService inventoryDocumentService)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();
            _inventoryDocumentService = inventoryDocumentService;

            _dbSet = dbContext.Set<FpReturnFromBuyerModel>();
            _dbDetailSet = dbContext.Set<FpReturnFromBuyerDetailModel>();
            _dbItemSet = dbContext.Set<FpReturnFromBuyerItemModel>();
        }

        public async Task<int> CreateAsync(FpReturnFromBuyerModel model)
        {
            do
            {
                model.Code = CodeGenerator.GenerateCode();
            }
            while (_dbSet.Any(d => d.Code.Equals(model.Code)));

            model.FlagForCreate(_identityService.Username, UserAgent);
            foreach (var detail in model.Details)
            {
                detail.FlagForCreate(_identityService.Username, UserAgent);
                foreach (var item in detail.Items)
                {
                    item.FlagForCreate(_identityService.Username, UserAgent);
                    _dbItemSet.Add(item);
                }
                _dbDetailSet.Add(detail);
            }
            _dbSet.Add(model);

            await CreateInventory(model, "IN");
            return await _dbContext.SaveChangesAsync();
        }

        private async Task CreateInventory(FpReturnFromBuyerModel model, string type)
        {
            var inventoryDocument = new InventoryDocument()
            {
                Date = model.Date,
                Items = GetInventoryItems(model.Details),
                ReferenceNo = model.Code,
                ReferenceType = "Return From Buyer",
                StorageCode = model.StorageCode,
                StorageId = model.StorageId,
                StorageName = model.StorageName,
                Type = type
            };

            await _inventoryDocumentService.Create(inventoryDocument);
        }

        private ICollection<InventoryDocumentItem> GetInventoryItems(ICollection<FpReturnFromBuyerDetailModel> details)
        {
            var result = new List<InventoryDocumentItem>();

            foreach (var detail in details)
            {
                foreach (var item in detail.Items)
                {
                    result.Add(new InventoryDocumentItem()
                    {
                        ProductCode = item.ProductCode,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.ReturnQuantity,
                        UomId = item.UOMId,
                        UomUnit = item.UOM
                    });
                }
            }

            return result;
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public FpReturnFromBuyerModel MapToModel(FpReturnFromBuyerViewModel viewModel)
        {
            return new FpReturnFromBuyerModel()
            {
                Active = viewModel.Active,
                BuyerCode = viewModel.Buyer.Code,
                BuyerId = viewModel.Buyer.Id,
                BuyerName = viewModel.Buyer.Name,
                Code = viewModel.Code,
                CodeProduct = viewModel.CodeProduct,
                CoverLetter = viewModel.CoverLetter,
                Date = viewModel.Date,
                Destination = viewModel.Destination,
                Details = MapDetailVMToModel(viewModel.Details),
                Id = viewModel.Id,
                SpkNo = viewModel.SpkNo,
                StorageCode = viewModel.Storage.code,
                StorageId = viewModel.Storage._id,
                StorageName = viewModel.Storage.name,
                _CreatedAgent = viewModel._CreatedAgent,
                _CreatedBy = viewModel._CreatedBy,
                _CreatedUtc = viewModel._CreatedUtc,
                _IsDeleted = viewModel._IsDeleted,
                _LastModifiedAgent = viewModel._LastModifiedAgent,
                _LastModifiedBy = viewModel._LastModifiedBy,
                _LastModifiedUtc = viewModel._LastModifiedUtc
            };
        }

        private List<FpReturnFromBuyerDetailModel> MapDetailVMToModel(IList<FpReturnFromBuyerDetailViewModel> details)
        {
            var result = new List<FpReturnFromBuyerDetailModel>();

            if (details != null && details.Count > 0)
                foreach (var detail in details)
                {
                    result.Add(new FpReturnFromBuyerDetailModel()
                    {
                        Active = detail.Active,
                        Id = detail.Id,
                        Items = MapItemVMToModel(detail.Items),
                        ProductionOrderId = detail.ProductionOrder.Id,
                        ProductionOrderNo = detail.ProductionOrder.OrderNo,
                        _CreatedAgent = detail._CreatedAgent,
                        _CreatedBy = detail._CreatedBy,
                        _CreatedUtc = detail._CreatedUtc,
                        _IsDeleted = detail._IsDeleted,
                        _LastModifiedAgent = detail._LastModifiedAgent,
                        _LastModifiedBy = detail._LastModifiedBy,
                        _LastModifiedUtc = detail._LastModifiedUtc
                    });
                }

            return result;
        }

        private ICollection<FpReturnFromBuyerItemModel> MapItemVMToModel(IList<FpReturnFromBuyerItemViewModel> items)
        {
            var result = new List<FpReturnFromBuyerItemModel>();

            if (items != null && items.Count > 0)
                foreach (var item in items)
                {
                    result.Add(new FpReturnFromBuyerItemModel()
                    {
                        Active = item.Active,
                        ColorWay = item.ColorWay,
                        DesignCode = item.DesignCode,
                        DesignNumber = item.DesignNumber,
                        Id = item.Id,
                        Length = item.Length,
                        ProductCode = item.ProductCode,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Remark = item.Remark,
                        ReturnQuantity = item.ReturnQuantity,
                        UOM = item.UOM,
                        UOMId = item.UOMId,
                        Weight = item.Weight,
                        _CreatedAgent = item._CreatedAgent,
                        _CreatedBy = item._CreatedBy,
                        _CreatedUtc = item._CreatedUtc,
                        _IsDeleted = item._IsDeleted,
                        _LastModifiedAgent = item._LastModifiedAgent,
                        _LastModifiedBy = item._LastModifiedBy,
                        _LastModifiedUtc = item._LastModifiedUtc
                    });
                }

            return result;
        }

        public FpReturnFromBuyerViewModel MapToViewModel(FpReturnFromBuyerModel model)
        {
            return new FpReturnFromBuyerViewModel()
            {
                Active = model.Active,
                Buyer = new BuyerViewModel()
                {
                    Code = model.BuyerCode,
                    Id = model.BuyerId,
                    Name = model.BuyerName
                },
                Code = model.Code,
                CodeProduct = model.CodeProduct,
                CoverLetter = model.CoverLetter,
                Date = model.Date,
                Destination = model.Destination,
                Details = MapDetailToViewModel(model.Details),
                Id = model.Id,
                SpkNo = model.SpkNo,
                Storage = new StorageIntegrationViewModel()
                {
                    code = model.StorageCode,
                    name = model.StorageName,
                    _id = model.StorageId
                },
                _CreatedAgent = model._CreatedAgent,
                _CreatedBy = model._CreatedBy,
                _CreatedUtc = model._CreatedUtc,
                _IsDeleted = model._IsDeleted,
                _LastModifiedAgent = model._LastModifiedAgent,
                _LastModifiedBy = model._LastModifiedBy,
                _LastModifiedUtc = model._LastModifiedUtc
            };
        }

        private List<FpReturnFromBuyerDetailViewModel> MapDetailToViewModel(ICollection<FpReturnFromBuyerDetailModel> details)
        {
            var result = new List<FpReturnFromBuyerDetailViewModel>();

            foreach (var detail in details)
            {
                result.Add(new FpReturnFromBuyerDetailViewModel()
                {
                    Active = detail.Active,
                    Id = detail.Id,
                    Items = MapItemToViewModel(detail.Items),
                    ProductionOrder = new ProductionOrderIntegrationViewModel()
                    {
                        Id = detail.ProductionOrderId,
                        OrderNo = detail.ProductionOrderNo
                    },
                    _CreatedAgent = detail._CreatedAgent,
                    _CreatedBy = detail._CreatedBy,
                    _CreatedUtc = detail._CreatedUtc,
                    _IsDeleted = detail._IsDeleted,
                    _LastModifiedAgent = detail._LastModifiedAgent,
                    _LastModifiedBy = detail._LastModifiedBy,
                    _LastModifiedUtc = detail._LastModifiedUtc
                });
            }

            return result;
        }

        private List<FpReturnFromBuyerItemViewModel> MapItemToViewModel(ICollection<FpReturnFromBuyerItemModel> items)
        {
            var result = new List<FpReturnFromBuyerItemViewModel>();

            foreach (var item in items)
            {
                result.Add(new FpReturnFromBuyerItemViewModel()
                {
                    Active = item.Active,
                    ColorWay = item.ColorWay,
                    DesignCode = item.DesignCode,
                    DesignNumber = item.DesignNumber,
                    Id = item.Id,
                    Length = item.Length,
                    ProductCode = item.ProductCode,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Remark = item.Remark,
                    ReturnQuantity = item.ReturnQuantity,
                    UOM = item.UOM,
                    UOMId = item.UOMId,
                    Weight = item.Weight,
                    _CreatedAgent = item._CreatedAgent,
                    _CreatedBy = item._CreatedBy,
                    _CreatedUtc = item._CreatedUtc,
                    _IsDeleted = item._IsDeleted,
                    _LastModifiedAgent = item._LastModifiedAgent,
                    _LastModifiedBy = item._LastModifiedBy,
                    _LastModifiedUtc = item._LastModifiedUtc
                });
            }

            return result;
        }

        public ReadResponse<FpReturnFromBuyerModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<FpReturnFromBuyerModel> Query = _dbSet;

            Query = Query.Where(w => !w.IsVoid);

            List<string> SearchAttributes = new List<string>()
            {
                "Code", "Destination", "BuyerName","SpkNo", "CoverLetter"
             };
            Query = QueryHelper<FpReturnFromBuyerModel>.Search(Query, SearchAttributes, keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Date", "Destination", "Buyer", "SpkNo", "CoverLetter"
            };
            Query = Query
                .Select(o => new FpReturnFromBuyerModel
                {
                    Id = o.Id,
                    Code = o.Code,
                    Date = o.Date,
                    BuyerCode = o.BuyerCode,
                    BuyerId = o.BuyerId,
                    BuyerName = o.BuyerName,
                    CoverLetter = o.CoverLetter,
                    Destination = o.Destination,
                    SpkNo = o.SpkNo
                });

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<FpReturnFromBuyerModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<FpReturnFromBuyerModel>.Order(Query, OrderDictionary);

            Pageable<FpReturnFromBuyerModel> pageable = new Pageable<FpReturnFromBuyerModel>(Query, page - 1, size);
            List<FpReturnFromBuyerModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<FpReturnFromBuyerModel>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public Task<FpReturnFromBuyerModel> ReadByIdAsync(int id)
        {
            return _dbSet.Include(i => i.Details).ThenInclude(ti => ti.Items).FirstOrDefaultAsync(f => f.Id.Equals(id));
        }

        public Task<int> UpdateAsync(int id, FpReturnFromBuyerModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<int> VoidDocumentById(int id)
        {
            var model = _dbSet.Include(i => i.Details).ThenInclude(ti => ti.Items).FirstOrDefault(f => f.Id.Equals(id));
            if (model == null)
                throw new Exception("Invalid Id");

            model.IsVoid = true;

            model.FlagForUpdate(_identityService.Username, UserAgent);

            _dbSet.Update(model);

            await CreateInventory(model, "OUT");

            return await _dbContext.SaveChangesAsync();
        }
    }
}
