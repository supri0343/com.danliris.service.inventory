using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction.Report
{
    public interface IGarmentWasteReportService
    {
        Task<List<GarmentWasteReportVM>> GetReport(DateTime DateFrom, DateTime DateTo, string type);
        Task<List<GarmentWasteRecapVM>> GetRecap(DateTime DateFrom, DateTime DateTo, string type, string transactionType);
    }
}
