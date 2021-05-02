using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices
{
    public interface IMaterialRequestNoteService : IBaseService<MaterialsRequestNote, MaterialsRequestNoteViewModel>
    {
        Task UpdateIsCompleted(int Id, MaterialsRequestNote Model);
        Tuple<List<MaterialsRequestNoteReportViewModel>, int> GetReport(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        void UpdateDistributedQuantity(int Id, MaterialsRequestNote Model);
    }
}
