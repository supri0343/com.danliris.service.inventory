using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftOverMutationViewModel;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftOverMutationServices
{
    public class GarmentLeftOverMutationService
    {
        private const string UserAgent = "GarmentLeftOverMutationService";
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public GarmentLeftOverMutationService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        //public Tuple<List<GarmentLeftOverMutationViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        //{
        //    var Query = GetReportQuery(dateFrom, dateTo, offset);

        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    if (OrderDictionary.Count.Equals(0))
        //    {
        //        Query = Query.OrderByDescending(b => b.ClassificationCode);
        //    }
        //    else
        //    {
        //        string Key = OrderDictionary.Keys.First();
        //        string OrderType = OrderDictionary[Key];
        //        Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
        //    }

        //    Pageable<GarmentLeftOverMutationViewModel> pageable = new Pageable<GarmentLeftOverMutationViewModel>(Query, page - 1, size);
        //    List<GarmentLeftOverMutationViewModel> Data = pageable.Data.ToList<GarmentLeftOverMutationViewModel>();

        //    //var localSalesNoteNo = Data.Where(t => t.LocalSalesNoteNo != null).Select(o => o.LocalSalesNoteNo).Distinct()

        //    int TotalData = pageable.TotalCount;



        //    return Tuple.Create(Data, TotalData);
        //}

        //public IQueryable<GarmentLeftOverMutationViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        //{
        //    DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
        //    DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

        //    #region BalanceStock
        //    var stock = from a in DbContext.GarmentLeftoverWarehouseBalanceStocks
        //                join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
        //                where a._IsDeleted == false
        //                && b._IsDeleted == false
        //                && a._CreatedUtc.Date < DateFrom.Date

        //    #endregion
        //}
    }
}
