using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Monitoring;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using System.IO;
namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Monitoring
{
    public class GarmentLeftoverWarehouseMonitoringBCService : IGarmentLeftoverWarehouseMonitoringBCService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseMonitoringService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseMonitoringBCService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public List<GarmentLeftoverWarehouseMonitoringViewModel> GetReportQuery(List<GarmentLeftoverWarehouseMonitoringParameterViewModel> param)
        {
            List<GarmentLeftoverWarehouseMonitoringViewModel> result = new List<GarmentLeftoverWarehouseMonitoringViewModel>();

            foreach(var data in param)
            {
                //join left over receipt fabric and left over receipt aval item and left join left over expenditure fabric and left over expenditure aval item
                var QueryFab = (from a in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                join c in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems on b.POSerialNumber equals c.PONo into expenditureItem
                                from c in expenditureItem.DefaultIfEmpty()
                                join d in DbContext.GarmentLeftoverWarehouseExpenditureFabrics on c.ExpenditureId equals d.Id into expenditure
                                from d in expenditure.DefaultIfEmpty()
                                where a._IsDeleted == false && b._IsDeleted == false
                                && (c == null || c._IsDeleted == false)
                                && (d == null || d._IsDeleted == false)
                                && a.UENNo == data.UENNo && b.POSerialNumber == data.PO
                                select new
                                {
                                    ReceiptNo = a.ReceiptNoteNo,
                                    ReceiptQty = b.Quantity,
                                    ExpenditureNo = d.ExpenditureNo,
                                    ExpenditureQty = 0,
                                    UENNo = a.UENNo,
                                    PO = b.POSerialNumber

                                })
                            //group then sum
                            .GroupBy(x => new { x.ReceiptNo, x.ExpenditureNo, x.UENNo, x.PO })
                            .Select(x => new GarmentLeftoverWarehouseMonitoringViewModel
                            {
                                ReceiptNo = x.Key.ReceiptNo,
                                ReceiptQty = Math.Round(x.Sum(y => y.ReceiptQty),2),
                                ExpenditureNo = x.Key.ExpenditureNo,
                                //ExpenditureQty = x.Sum(y => y.ExpenditureQty),
                                ExpenditureQty = 0,
                                UENNo = x.Key.UENNo,
                                PO = x.Key.PO
                            }).ToList();
                

                //create query for accecory
                var QueryAcc = (from a in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                join c in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems on b.POSerialNumber equals c.PONo into expenditureItem
                                from c in expenditureItem.DefaultIfEmpty()
                                join d in DbContext.GarmentLeftoverWarehouseExpenditureAccessories on c.ExpenditureId equals d.Id into expenditure
                                from d in expenditure.DefaultIfEmpty()
                                where a._IsDeleted == false && b._IsDeleted == false
                                && (c == null || c._IsDeleted == false)
                                && (d == null || d._IsDeleted == false)
                                && a.UENNo == data.UENNo && b.POSerialNumber == data.PO
                                select new 
                                {
                                    ReceiptNo = a.InvoiceNoReceive,
                                    ReceiptQty = b.Quantity,
                                    ExpenditureNo = d.ExpenditureNo,
                                    ExpenditureQty = 0,
                                    UENNo = a.UENNo,
                                    PO = b.POSerialNumber
                                }).GroupBy(x => new { x.ReceiptNo, x.ExpenditureNo, x.UENNo, x.PO })
                            .Select(x => new GarmentLeftoverWarehouseMonitoringViewModel
                            {
                                ReceiptNo = x.Key.ReceiptNo,
                                ReceiptQty = Math.Round(x.Sum(y => y.ReceiptQty),2),
                                ExpenditureNo = x.Key.ExpenditureNo,
                                //ExpenditureQty = x.Sum(y => y.ExpenditureQty),
                                ExpenditureQty = 0,
                                UENNo = x.Key.UENNo,
                                PO = x.Key.PO
                            }).ToList();

                foreach(var a in QueryFab)
                {
                    a.ExpenditureQty = Math.Round(DbContext.GarmentLeftoverWarehouseExpenditureFabricItems.Where(s => s.PONo == a.PO).Sum(r => r.Quantity),2);
                }

                foreach(var a in QueryAcc)
                {
                    a.ExpenditureQty = Math.Round(DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems.Where(s => s.PONo == a.PO).Sum(r => r.Quantity),2);
                }

                //union fabric and acc
                var Query = QueryFab.Union(QueryAcc);

                result.AddRange(Query);
            }

            return result;
        }
    }
}
