using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure;
using Com.Moonlay.NetCore.Lib;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure
{
    public class GarmentLeftoverWarehouseReportExpenditureService : IGarmentLeftoverWarehouseReportExpenditureService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReportExpenditureService";
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;
        private readonly string GarmentShippingLocalCoverLetterUri;

        public GarmentLeftoverWarehouseReportExpenditureService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = serviceProvider.GetService<IIdentityService>();
            GarmentShippingLocalCoverLetterUri = APIEndpoint.PackingInventory + "garment-shipping/local-cover-letters";
        }

        public Tuple<List<GarmentLeftoverWarehouseReportExpenditureViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string receiptType, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, receiptType, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<GarmentLeftoverWarehouseReportExpenditureViewModel> pageable = new Pageable<GarmentLeftoverWarehouseReportExpenditureViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseReportExpenditureViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseReportExpenditureViewModel>();

            var localSalesNoteNo = Data.Where(t => t.LocalSalesNoteNo != null).Select(o => o.LocalSalesNoteNo).Distinct();


            Data.ForEach(c => {
                if(!String.IsNullOrEmpty(c.LocalSalesNoteNo))
                {
                    var salesNotes = GetBcFromShipping(c.LocalSalesNoteNo);
                    if (salesNotes["noteNo"].ToString() == c.LocalSalesNoteNo)
                    {
                        c.BCNo = salesNotes["bcNo"].ToString();
                        c.BCDate = DateTimeOffset.Parse(salesNotes["bcDate"].ToString());
                        
                    }

                }
                
            });

        

            int TotalData = pageable.TotalCount;



            return Tuple.Create(Data, TotalData);
        }

        public IQueryable<GarmentLeftoverWarehouseReportExpenditureViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, string receiptType, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;



            var Query = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                         // ExpenditureAcessoriesItem
                         join b in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems on a.Id equals b.ExpenditureId
                         join c in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on b.PONo equals c.POSerialNumber
                         //Conditions
                         where a._IsDeleted == false
                             && a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                             && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                         select new GarmentLeftoverWarehouseReportExpenditureViewModel
                         {
                             ExpenditureNo = a.ExpenditureNo,
                             ExpenditureDate = a.ExpenditureDate,
                             ExpenditureDestination = a.ExpenditureDestination,
                             DescriptionOfPurpose = a.ExpenditureDestination == "JUAL LOKAL" ? a.BuyerName : a.ExpenditureDestination == "UNIT" ? a.UnitExpenditureCode : a.ExpenditureDestination == "LAIN-LAIN" ? a.EtcRemark: "SAMPLE",
                             PONo = b.PONo,
                             Product = new ProductViewModel
                             {
                                  Id = Convert.ToString(c.ProductId),
                                  Code = c.ProductCode,
                                  Name = c.ProductName,
                             },
                             ProductRemark = c.ProductRemark,
                             Quantity = b.Quantity,
                             Uom = new UomViewModel
                             {
                                 Id = Convert.ToString(b.UomId),
                                 Unit = b.UomUnit
                             },
                             LocalSalesNoteNo = a.LocalSalesNoteNo,
                             BCType = "BC 25",
                             _LastModifiedUtc = a._LastModifiedUtc
                         });


            return Query;
        }

        private Dictionary<string, Object> GetBcFromShipping(string localSalesNoteNo)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            Dictionary<string, object> filterLocalCoverLetter = new Dictionary<string, object> { { "NoteNo", localSalesNoteNo } };
            var filter = JsonConvert.SerializeObject(filterLocalCoverLetter);
            var responseLocalCoverLetter = httpService.GetAsync($"{GarmentShippingLocalCoverLetterUri}?filter=" + filter).Result.Content.ReadAsStringAsync();

            Dictionary<string, object> resultLocalCoverLetter = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseLocalCoverLetter.Result);
            var jsonLocalCoverLetter = resultLocalCoverLetter.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> dataLocalCoverLetter = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonLocalCoverLetter.ToString())[0];
            return dataLocalCoverLetter;
        }
    }
}
