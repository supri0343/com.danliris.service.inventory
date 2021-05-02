using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FpRegradingResultDocs;
using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs
{
    public interface IFpRegradingResultDocsService : IBaseService<Models.FpRegradingResultDocs, FpRegradingResultDocsViewModel>
    {
        Tuple<List<Models.FpRegradingResultDocs>, int, Dictionary<string, string>, List<string>> ReadNo(string Keyword = null, string Filter = "{}", int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null);
        int UpdateIsReturnedToPurchasing(int fPRegradingResultDocsId, bool flag);
        Tuple<List<FpRegradingResultDocsReportViewModel>, int> GetReport(string unitName, string code, string productName, bool? isReturn, bool? isReturnedToPurchasing, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order);
    }
}
