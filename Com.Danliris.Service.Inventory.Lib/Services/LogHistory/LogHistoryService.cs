using com.Danliris.Service.Inventory.Lib.Models.LogHistory;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.LogHistories
{
    public class LogHistoryService : ILogHistoryService
    {
        private InventoryDbContext DbContext;
        private DbSet<LogHistory> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;
        public LogHistoryService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<LogHistory>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public async void CreateAsync(string division, string activity)
        {
            LogHistory model = new LogHistory
            {
                Division = division,
                Activity = activity,
                CreatedDate = DateTime.Now,
                CreatedBy = IdentityService.Username
            };

            DbSet.Add(model);
        }

        public async Task<List<LogHistoryViewModel>> GetReportQuery(DateTime dateFrom,DateTime dateTo)
        {
            var queryInventory = DbSet.Where(x => x.CreatedDate.AddHours(7).Date >= dateFrom.Date && x.CreatedDate.AddHours(7).Date <= dateTo.Date)
                .Select(x => new LogHistoryViewModel
                {
                    Activity = x.Activity,
                    Division = x.Division,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate.AddHours(7)
                });

            var queryGarment = await GetLogHistoryGarment(dateFrom, dateTo);
            var queryPackingInven = await GetLogHistoryPackingInventory(dateFrom, dateTo);
            var queryPurchasing = await GetLogHistoryPurchasing(dateFrom, dateTo);
            var querySales = await GetLogHistorySales(dateFrom, dateTo);


            var QueryUnion = queryInventory.Union(queryPackingInven).Union(queryPurchasing).Union(querySales).Union(queryGarment).OrderBy(x => x.Division).ThenBy(x => x.CreatedDate);

            //var QueryUnion = queryInventory;

            List<LogHistoryViewModel> Result = new List<LogHistoryViewModel>();

            foreach (var x in QueryUnion)
            {
                var dataToPush = new LogHistoryViewModel
                {
                    Activity = x.Activity,
                    Division = x.Division,
                    CreatedBy = x.CreatedBy,
                    Date = x.CreatedDate.ToString("dd/MM/yyyy"),
                    Time = x.CreatedDate.ToString("HH:mm"),
                };

                Result.Add(dataToPush);
            }

            return Result;
        }

        public async Task<Tuple<List<LogHistoryViewModel>, int>> GetMonitoring(DateTime dateFrom, DateTime dateTo, int page, int size)
        {
            var Query = await GetReportQuery(dateFrom, dateTo);

         
            Pageable<LogHistoryViewModel> pageable = new Pageable<LogHistoryViewModel>(Query, page - 1, size);
            List<LogHistoryViewModel> Data = pageable.Data.ToList<LogHistoryViewModel>();

            int TotalData = pageable.TotalCount;
            return Tuple.Create(Data, TotalData);
        }

        public async Task<MemoryStream> GenerateExcelReport(DateTime dateFrom, DateTime dateTo)
        {
            var Query = await GetReportQuery(dateFrom, dateTo);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bagian (Divisi)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "User", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Waktu", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jam", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Aktivitas", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 1;
                foreach (var item in Query)
                {
                    //var date = item.CreatedDate.ToString("dd/MM/yyyy");
                    //var time = item.CreatedDate.ToString("HH:mm");
                    result.Rows.Add(index++, item.Division,item.CreatedBy, item.Date, item.Time, item.Activity);

                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "Laporan Aktivitas (Log) User";
                worksheet.Cells["A" + 1 + ":F" + 1 + ""].Merge = true;
                worksheet.Cells["A2"].Value = "Periode " + dateFrom.ToString("dd-MM-yyyy") + " s/d " + dateTo.ToString("dd-MM-yyyy");
                worksheet.Cells["A" + 2 + ":F" + 2 + ""].Merge = true;

                worksheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;

                worksheet.Cells["A4"].LoadFromDataTable(result, true);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var stream = new MemoryStream();

                package.SaveAs(stream);

                return stream;
            }
        }

        private async Task<List<LogHistoryViewModel>> GetLogHistoryPackingInventory(DateTime from, DateTime to)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var httpContent = new StringContent(JsonConvert.SerializeObject( new { dateFrom = from, dateTo = to }), Encoding.UTF8, "application/json");

            //var httpResponse = await httpService.SendAsync(HttpMethod.Get, APIEndpoint.PackingInventory + "log-history", httpContent);
            var httpResponse = await httpService.GetAsync(APIEndpoint.PackingInventory + "log-history?dateFrom=" + from.Date + "&dateTo=" + to.Date);

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<LogHistoryViewModel> viewModel;

                viewModel = JsonConvert.DeserializeObject<List<LogHistoryViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<LogHistoryViewModel> viewModel = new List<LogHistoryViewModel>();
                return viewModel;
            }
        }

        private async Task<List<LogHistoryViewModel>> GetLogHistoryPurchasing(DateTime from, DateTime to)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var httpContent = new StringContent(JsonConvert.SerializeObject(new { dateFrom = from, dateTo = to }), Encoding.UTF8, "application/json");

            //var httpResponse = await httpService.SendAsync(HttpMethod.Get, APIEndpoint.PackingInventory + "log-history", httpContent);
            var httpResponse = await httpService.GetAsync(APIEndpoint.Purchasing + "log-history?dateFrom=" + from.Date + "&dateTo=" + to.Date);

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<LogHistoryViewModel> viewModel;

                viewModel = JsonConvert.DeserializeObject<List<LogHistoryViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<LogHistoryViewModel> viewModel = new List<LogHistoryViewModel>();
                return viewModel;
            }
        }

        private async Task<List<LogHistoryViewModel>> GetLogHistorySales(DateTime from, DateTime to)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var httpContent = new StringContent(JsonConvert.SerializeObject(new { dateFrom = from, dateTo = to }), Encoding.UTF8, "application/json");

            //var httpResponse = await httpService.SendAsync(HttpMethod.Get, APIEndpoint.PackingInventory + "log-history", httpContent);
            var httpResponse = await httpService.GetAsync(APIEndpoint.Sales + "log-history?dateFrom=" + from.Date + "&dateTo=" + to.Date);

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<LogHistoryViewModel> viewModel;

                viewModel = JsonConvert.DeserializeObject<List<LogHistoryViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<LogHistoryViewModel> viewModel = new List<LogHistoryViewModel>();
                return viewModel;
            }
        }

        private async Task<List<LogHistoryViewModel>> GetLogHistoryGarment(DateTime from, DateTime to)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var httpContent = new StringContent(JsonConvert.SerializeObject(new { dateFrom = from, dateTo = to }), Encoding.UTF8, "application/json");

            //var httpResponse = await httpService.SendAsync(HttpMethod.Get, APIEndpoint.PackingInventory + "log-history", httpContent);
            var httpResponse = await httpService.GetAsync(APIEndpoint.GarmentProduction + "log-history?dateFrom=" + from.Date + "&dateTo=" + to.Date);

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<LogHistoryViewModel> viewModel;

                viewModel = JsonConvert.DeserializeObject<List<LogHistoryViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<LogHistoryViewModel> viewModel = new List<LogHistoryViewModel>();
                return viewModel;
            }
        }
    }
}
