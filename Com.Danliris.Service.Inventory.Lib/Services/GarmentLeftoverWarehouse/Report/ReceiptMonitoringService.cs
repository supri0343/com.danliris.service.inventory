using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Migrations;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using Com.Danliris.Service.Inventory.Lib.ViewModels;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report
{
    public class ReceiptMonitoringService : IReceiptMonitoringService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptFabricMonitoringService";

        public InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptFabric> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly string GarmentCustomsUri;

        public ReceiptMonitoringService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptFabric>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            //StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));

            GarmentCustomsUri = APIEndpoint.Purchasing + "garment-beacukai/";
        }

        public int TotalCountReport { get; set; } = 0;
        private List<ReceiptMonitoringViewModel> GetFabricReceiptMonitoringQuery(DateTime? dateFrom, DateTime? dateTo, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<ReceiptMonitoringViewModel> listData = new List<ReceiptMonitoringViewModel>();

            var join = from a in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                       join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                       where a._IsDeleted==false    
                       && a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
                       && a.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                       select new
                       {
                           fabricId = a.Id,
                           itemId = b.Id,
                           date= a.ReceiptDate
                       };

            TotalCountReport = join.Distinct().OrderByDescending(o => o.date).Count();
            var queryResult = join.Distinct().OrderByDescending(o => o.date).Skip((page - 1) * size).Take(size).ToList();

            var fabricIds = queryResult.Select(s => s.fabricId).Distinct().ToList();
            var fabrics = DbContext.GarmentLeftoverWarehouseReceiptFabrics.Where(w => fabricIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptNoteNo, s.ReceiptDate, s.UnitFromCode, s.UENNo }).ToList();
            var itemIds = queryResult.Select(s => s.itemId).Distinct().ToList();
            var items = DbContext.GarmentLeftoverWarehouseReceiptFabricItems.Where(w => itemIds.Contains(w.Id)).Select(s => new { s.Id, s.POSerialNumber, s.ProductCode, s.ProductName, s.Quantity, s.UomUnit, s.ProductRemark, s.FabricRemark }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in queryResult)
            {
                var fabric = fabrics.FirstOrDefault(f => f.Id.Equals(item.fabricId));
                var fabricItem = items.FirstOrDefault(f => f.Id.Equals(item.itemId));

                ReceiptMonitoringViewModel vm = new ReceiptMonitoringViewModel();

                vm.ProductRemark = fabricItem.ProductRemark;
                vm.Product = new ProductViewModel { Code = fabricItem.ProductCode, Name = fabricItem.ProductName };
                vm.Quantity = fabricItem.Quantity;
                vm.Uom = new UomViewModel { Unit = fabricItem.UomUnit };
                vm.UnitFrom = new UnitViewModel { Code = fabric.UnitFromCode };
                vm.ReceiptDate = fabric.ReceiptDate;
                vm.ReceiptNoteNo = fabric.ReceiptNoteNo;
                vm.POSerialNumber = fabricItem.POSerialNumber;
                vm.index = i;
                vm.UENNo = fabric.UENNo;
                vm.FabricRemark = fabricItem.FabricRemark;

                listData.Add(vm);
                i++;

            }
            return listData;
        }

        public Tuple<List<ReceiptMonitoringViewModel>, int> GetFabricReceiptMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Data = GetFabricReceiptMonitoringQuery(dateFrom, dateTo, offset, page, size);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }

        private List<ReceiptMonitoringViewModel> GetAccessoriesReceiptMonitoringQuery(DateTime? dateFrom, DateTime? dateTo, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<ReceiptMonitoringViewModel> listData = new List<ReceiptMonitoringViewModel>();

            var join = from a in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                       join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                       where a._IsDeleted == false
                       && a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date
                       && a.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                       select new
                       {
                           accId = a.Id,
                           itemId = b.Id,
                           date = a.StorageReceiveDate
                       };

            TotalCountReport = join.Distinct().OrderByDescending(o => o.date).Count();
            var queryResult = join.Distinct().OrderByDescending(o => o.date).Skip((page - 1) * size).Take(size).ToList();

            var accIds = queryResult.Select(s => s.accId).Distinct().ToList();
            var accs = DbContext.GarmentLeftoverWarehouseReceiptAccessories.Where(w => accIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceNoReceive, s.StorageReceiveDate, s.RequestUnitCode, s.UENNo }).ToList();
            var itemIds = queryResult.Select(s => s.itemId).Distinct().ToList();
            var items = DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems.Where(w => itemIds.Contains(w.Id)).Select(s => new { s.Id, s.POSerialNumber, s.ProductCode, s.ProductName, s.Quantity, s.UomUnit, s.ProductRemark }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in queryResult)
            {
                var acc = accs.FirstOrDefault(f => f.Id.Equals(item.accId));
                var accItem = items.FirstOrDefault(f => f.Id.Equals(item.itemId));

                ReceiptMonitoringViewModel vm = new ReceiptMonitoringViewModel();

                vm.ProductRemark = accItem.ProductRemark;
                vm.Product = new ProductViewModel { Code = accItem.ProductCode, Name = accItem.ProductName };
                vm.Quantity = accItem.Quantity;
                vm.Uom = new UomViewModel { Unit = accItem.UomUnit };
                vm.UnitFrom = new UnitViewModel { Code = acc.RequestUnitCode };
                vm.ReceiptDate = acc.StorageReceiveDate;
                vm.ReceiptNoteNo = acc.InvoiceNoReceive;
                vm.POSerialNumber = accItem.POSerialNumber;
                vm.index = i;
                vm.UENNo = acc.UENNo;

                listData.Add(vm);
                i++;

            }
            return listData;
        }

        public Tuple<List<ReceiptMonitoringViewModel>, int> GetAccessoriesReceiptMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Data = GetAccessoriesReceiptMonitoringQuery(dateFrom, dateTo, offset, page, size);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }

        //private async Task UpdateUnitExpenditureNoteIsReceived(List<string> POSerialNumber)
        //{

        //    var stringContentRequest = JsonConvert.SerializeObject(new List<object>
        //    {
        //        new { op = "replace", path = "/IsReceived", value = IsReceived }
        //    });
        //    var httpContentRequest = new StringContent(stringContentRequest, Encoding.UTF8, General.JsonMediaType);

        //    var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

        //    var response = await httpService.PatchAsync(GarmentUnitReceiptNoteUri + UENId, httpContentRequest);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var contentResponse = await response.Content.ReadAsStringAsync();
        //        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentResponse) ?? new Dictionary<string, object>();

        //        throw new Exception(string.Concat("Error from '", GarmentUnitReceiptNoteUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
        //    }
        //}
    }
}
