using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction.Report
{
    public class GarmentWasteReportService : IGarmentWasteReportService
    {
        private InventoryDbContext DbContext;
        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        // Receipt
        private DbSet<GarmentReceiptWasteProductions> _receiptWaste;
        private DbSet<GarmentReceiptWasteProductionItems> _receiptWasteItem;


        // Expenditure
        private DbSet<GarmentExpenditureWasteProductions> _expenditureWaste;
        private DbSet<GarmentExpenditureWasteProductionItems> _expenditureWasteItem;

        public GarmentWasteReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            _receiptWaste = DbContext.Set<GarmentReceiptWasteProductions>();
            _receiptWasteItem = DbContext.Set<GarmentReceiptWasteProductionItems>();

            _expenditureWaste = DbContext.Set<GarmentExpenditureWasteProductions>();
            _expenditureWasteItem = DbContext.Set<GarmentExpenditureWasteProductionItems>();
        }

        class monitoringView
        {
            public string classificationName { get; set; }
            public string unitQtyName { get; set; }
            public double saldoAwal { get; set; }
            public double pemasukan { get; set; }
            public double pengeluaran { get; set; }
            public double saldoAkhir { get; set; }
        }

        public async Task<List<GarmentWasteReportVM>>GetReport(DateTime DateFrom, DateTime DateTo, string type)
        {
            DateTimeOffset dateFrom = new DateTimeOffset(DateFrom, new TimeSpan(0, 0, 0));
            DateTimeOffset dateTo = new DateTimeOffset(DateTo, new TimeSpan(0, 0, 0));

            List<GarmentWasteReportVM> getMutationScrapDtos = new List<GarmentWasteReportVM>();

            var SAScrapIN = (from a in _receiptWaste 
                             join b in _receiptWasteItem on a.Id equals b.GarmentReceiptWasteId
                             where a.ReceiptDate.AddHours(IdentityService.TimezoneOffset).Date < dateFrom.Date && a._IsDeleted == false && b._IsDeleted == false
                             && a.WasteType == (string.IsNullOrWhiteSpace(type) ? a.WasteType : type)
                             select new monitoringView
                             {
                                 //classificationCode = c.Code,
                                 classificationName = a.WasteType,
                                 saldoAwal = b.Quantity,
                                 pemasukan = 0,
                                 pengeluaran = 0,
                                 //penyesuaian = 0,
                                 saldoAkhir = 0,
                                 //selisih = 0,
                                 //stockOpname = 0,
                                 unitQtyName = "KG"
                             }).GroupBy(x => new { /*x.classificationCode,*/ x.classificationName, x.unitQtyName }, (key, group) => new monitoringView
                             {
                                 //classificationCode = key.classificationCode,
                                 classificationName = key.classificationName,
                                 saldoAwal = group.Sum(x => x.saldoAwal),
                                 pemasukan = group.Sum(x => x.pemasukan),
                                 pengeluaran = group.Sum(x => x.pengeluaran),
                                 //penyesuaian = group.Sum(x => x.penyesuaian),
                                 saldoAkhir = group.Sum(x => x.saldoAkhir),
                                 //selisih = group.Sum(x => x.selisih),
                                 //stockOpname = group.Sum(x => x.stockOpname),
                                 unitQtyName = key.unitQtyName

                             });

            var SAScrapOut = (from a in _expenditureWaste
                             join b in _expenditureWasteItem on a.Id equals b.GarmentExpenditureWasteId
                             where a.ExpenditureDate.AddHours(IdentityService.TimezoneOffset).Date < dateFrom.Date && a._IsDeleted == false && b._IsDeleted == false
                             && a.WasteType == (string.IsNullOrWhiteSpace(type) ? a.WasteType : type)
                             select new monitoringView
                             {
                                 //classificationCode = c.Code,
                                 classificationName = a.WasteType,
                                 saldoAwal = - b.Quantity,
                                 pemasukan = 0,
                                 pengeluaran = 0,
                                 //penyesuaian = 0,
                                 saldoAkhir = 0,
                                 //selisih = 0,
                                 //stockOpname = 0,
                                 unitQtyName = "KG"
                             }).GroupBy(x => new { /*x.classificationCode,*/ x.classificationName, x.unitQtyName }, (key, group) => new monitoringView
                             {
                                 //classificationCode = key.classificationCode,
                                 classificationName = key.classificationName,
                                 saldoAwal = group.Sum(x => x.saldoAwal),
                                 pemasukan = group.Sum(x => x.pemasukan),
                                 pengeluaran = group.Sum(x => x.pengeluaran),
                                 //penyesuaian = group.Sum(x => x.penyesuaian),
                                 saldoAkhir = group.Sum(x => x.saldoAkhir),
                                 //selisih = group.Sum(x => x.selisih),
                                 //stockOpname = group.Sum(x => x.stockOpname),
                                 unitQtyName = key.unitQtyName

                             });

            var SA = SAScrapIN.Concat(SAScrapOut).AsEnumerable();
            var SaldoAwal = SA.GroupBy(x => new { /*x.classificationCode,*/ x.classificationName, x.unitQtyName }, (key, group) => new monitoringView
            {
                //classificationCode = key.classificationCode,
                classificationName = key.classificationName,
                saldoAwal = group.Sum(x => x.saldoAwal),
                pemasukan = group.Sum(x => x.pemasukan),
                pengeluaran = group.Sum(x => x.pengeluaran),
                //penyesuaian = group.Sum(x => x.penyesuaian),
                saldoAkhir = group.Sum(x => x.saldoAkhir),
                //selisih = group.Sum(x => x.selisih),
                //stockOpname = group.Sum(x => x.stockOpname),
                unitQtyName = key.unitQtyName

            });

            var FilterdScrapIN = (from a in _receiptWaste
                             join b in _receiptWasteItem on a.Id equals b.GarmentReceiptWasteId
                             where 
                             a.ReceiptDate.AddHours(IdentityService.TimezoneOffset).Date >= dateFrom.Date
                             && a.ReceiptDate.AddHours(IdentityService.TimezoneOffset).Date <= dateTo.Date
                             && a._IsDeleted == false && b._IsDeleted == false
                             && a.WasteType == (string.IsNullOrWhiteSpace(type) ? a.WasteType : type)
                             select new monitoringView
                             {
                                 //classificationCode = c.Code,
                                 classificationName = a.WasteType,
                                 saldoAwal = 0,
                                 pemasukan = b.Quantity,
                                 pengeluaran = 0,
                                 //penyesuaian = 0,
                                 saldoAkhir = 0,
                                 //selisih = 0,
                                 //stockOpname = 0,
                                 unitQtyName = "KG"
                             }).GroupBy(x => new { /*x.classificationCode,*/ x.classificationName, x.unitQtyName }, (key, group) => new monitoringView
                             {
                                 //classificationCode = key.classificationCode,
                                 classificationName = key.classificationName,
                                 saldoAwal = group.Sum(x => x.saldoAwal),
                                 pemasukan = group.Sum(x => x.pemasukan),
                                 pengeluaran = group.Sum(x => x.pengeluaran),
                                 //penyesuaian = group.Sum(x => x.penyesuaian),
                                 saldoAkhir = group.Sum(x => x.saldoAkhir),
                                 //selisih = group.Sum(x => x.selisih),
                                 //stockOpname = group.Sum(x => x.stockOpname),
                                 unitQtyName = key.unitQtyName

                             });

            var FilterdScrapOut = (from a in _expenditureWaste
                                   join b in _expenditureWasteItem on a.Id equals b.GarmentExpenditureWasteId
                                  where
                                  a.ExpenditureDate.AddHours(IdentityService.TimezoneOffset).Date >= dateFrom.Date
                                  && a.ExpenditureDate.AddHours(IdentityService.TimezoneOffset).Date <= dateTo.Date
                                  && a._IsDeleted == false && b._IsDeleted == false
                                  && a.WasteType == (string.IsNullOrWhiteSpace(type) ? a.WasteType : type)
                                  select new monitoringView
                                  {
                                      //classificationCode = c.Code,
                                      classificationName = a.WasteType,
                                      saldoAwal = 0,
                                      pemasukan = 0,
                                      pengeluaran = b.Quantity,
                                      //penyesuaian = 0,
                                      saldoAkhir = 0,
                                      //selisih = 0,
                                      //stockOpname = 0,
                                      unitQtyName = "KG"
                                  }).GroupBy(x => new { /*x.classificationCode,*/ x.classificationName, x.unitQtyName }, (key, group) => new monitoringView
                                  {
                                      //classificationCode = key.classificationCode,
                                      classificationName = key.classificationName,
                                      saldoAwal = group.Sum(x => x.saldoAwal),
                                      pemasukan = group.Sum(x => x.pemasukan),
                                      pengeluaran = group.Sum(x => x.pengeluaran),
                                      //penyesuaian = group.Sum(x => x.penyesuaian),
                                      saldoAkhir = group.Sum(x => x.saldoAkhir),
                                      //selisih = group.Sum(x => x.selisih),
                                      //stockOpname = group.Sum(x => x.stockOpname),
                                      unitQtyName = key.unitQtyName

                                  });

            var SAkhir = SaldoAwal.Concat(FilterdScrapIN).Concat(FilterdScrapOut).AsEnumerable();
            var Saldokhir = SAkhir.GroupBy(x => new { /*x.classificationCode,*/ x.classificationName, x.unitQtyName }, (key, group) => new monitoringView
            {
                //classificationCode = key.classificationCode,
                classificationName = key.classificationName,
                saldoAwal = group.Sum(x => x.saldoAwal),
                pemasukan = group.Sum(x => x.pemasukan),
                pengeluaran = group.Sum(x => x.pengeluaran),
                //penyesuaian = group.Sum(x => x.penyesuaian),
                saldoAkhir = group.Sum(x => x.saldoAwal) + group.Sum(x => x.pemasukan) - group.Sum(x => x.pengeluaran),
                //selisih = group.Sum(x => x.selisih),
                //stockOpname = group.Sum(x => x.stockOpname),
                unitQtyName = key.unitQtyName

            });

            foreach (var a in Saldokhir)
            {
                getMutationScrapDtos.Add(new GarmentWasteReportVM
                {
                    //ClassificationCode = a.classificationCode,
                    ClassificationName = a.classificationName,
                    SaldoAwal = Math.Round(a.saldoAwal, 2),
                    Pemasukan = Math.Round(a.pemasukan, 2),
                    Pengeluaran = Math.Round(a.pengeluaran, 2),
                    //Penyesuaian = Math.Round(a.penyesuaian, 2),
                    SaldoAkhir = Math.Round(a.saldoAkhir, 2),
                    //Selisih = a.selisih,
                    //StockOpname = a.stockOpname,
                    UnitQtyName = a.unitQtyName
                });
            }

            return getMutationScrapDtos;
        }

        public async Task<List<GarmentWasteRecapVM>> GetRecap(DateTime DateFrom, DateTime DateTo, string type, string transactionType)
        {
            DateTimeOffset dateFrom = new DateTimeOffset(DateFrom, new TimeSpan(0, 0, 0));
            DateTimeOffset dateTo = new DateTimeOffset(DateTo, new TimeSpan(0, 0, 0));

            switch (transactionType)
            {
                case "PEMASUKAN":
                    // Receipt
                    var receipt = await (from a in _receiptWaste
                                         join b in _receiptWasteItem on a.Id equals b.GarmentReceiptWasteId
                                         where a.ReceiptDate.AddHours(IdentityService.TimezoneOffset).Date >= dateFrom.Date
                                         && a.ReceiptDate.AddHours(IdentityService.TimezoneOffset).Date <= dateTo.Date
                                         && a.WasteType == (string.IsNullOrWhiteSpace(type) ? a.WasteType : type)
                                         && a._IsDeleted == false && b._IsDeleted == false
                                         select new GarmentWasteRecapVM
                                         {
                                             BonNo = a.GarmentReceiptWasteNo,
                                             Date = a.ReceiptDate.AddHours(IdentityService.TimezoneOffset),
                                             ReceiptSource = a.SourceName,
                                             Quantity = b.Quantity,
                                             Uom = "KG",
                                         })
                                         .GroupBy(x => new { x.BonNo, x.Date, x.ReceiptSource, x.Uom }, (key, group) => new GarmentWasteRecapVM
                                         {
                                             BonNo = key.BonNo,
                                             Date = key.Date,
                                             ReceiptSource = key.ReceiptSource,
                                             Quantity = Math.Round(group.Sum(x => x.Quantity),2),
                                             Uom = key.Uom,
                                         }).ToListAsync();

                    return receipt;

                case "PENGELUARAN":
                    // Expenditure
                    var expenditure = await (from a in _expenditureWaste
                                             join b in _expenditureWasteItem on a.Id equals b.GarmentExpenditureWasteId
                                             where a.ExpenditureDate.AddHours(IdentityService.TimezoneOffset).Date >= dateFrom.Date
                                             && a.ExpenditureDate.AddHours(IdentityService.TimezoneOffset).Date <= dateTo.Date
                                             && a.WasteType == (string.IsNullOrWhiteSpace(type) ? a.WasteType : type)
                                             && a._IsDeleted == false && b._IsDeleted == false
                                             select new GarmentWasteRecapVM
                                             {
                                                 BonNo = a.GarmentExpenditureWasteNo,
                                                 Date = a.ExpenditureDate.AddHours(IdentityService.TimezoneOffset),
                                                 Quantity = b.Quantity,
                                                 Remark = a.Description,
                                                 Uom = "KG",
                                             })
                                             .GroupBy(x => new { x.BonNo, x.Date, x.Remark, x.Uom }, (key, group) => new GarmentWasteRecapVM
                                             {
                                                 BonNo = key.BonNo,
                                                 Date = key.Date,
                                                 Quantity = Math.Round(group.Sum(x => x.Quantity),2),
                                                 Remark = key.Remark,
                                                 Uom = key.Uom,
                                             }).ToListAsync();

                    return expenditure;

                default:
                    return new List<GarmentWasteRecapVM>();
            }
        }
    }
}
