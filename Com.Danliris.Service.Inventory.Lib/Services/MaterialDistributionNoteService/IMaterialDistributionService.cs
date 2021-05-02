using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService
{
    public interface IMaterialDistributionService : IBaseService<MaterialDistributionNote, MaterialDistributionNoteViewModel>
    {
        bool UpdateIsApprove(List<int> Ids);
        Tuple<List<MaterialDistributionNoteReportViewModel>, int> GetReport(string unitId, string type, DateTime date, int page, int size, string Order, int offset);
        List<MaterialDistributionNoteReportViewModel> GetPdfReport(string unitId, string unitName, string type, DateTime date, int offset);
    }
}
