using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService
{
    public interface IStockTransferNoteService : IBaseService<StockTransferNote, StockTransferNoteViewModel>
    {
        Tuple<List<StockTransferNote>, int, Dictionary<string, string>, List<string>> ReadModelByNotUser(int Page, int Size, string Order, List<string> Select, string Keyword, string Filter);
        Task<bool> UpdateIsApprove(int Id);
    }
}
