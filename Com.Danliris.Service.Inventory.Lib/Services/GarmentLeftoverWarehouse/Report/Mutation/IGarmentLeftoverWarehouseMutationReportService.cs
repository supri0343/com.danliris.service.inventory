using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Mutation
{
    public interface IGarmentLeftoverWarehouseMutationReportService
    {
        Tuple<List<GarmentLeftoverWarehouseMutationReportViewModel>, int> GetMutation(DateTime? dateFrom, DateTime? dateTo, int page, int size);
        MemoryStream GenerateExcelMutation(DateTime dateFrom, DateTime dateTo);
    }
}
