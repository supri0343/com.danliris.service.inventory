using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.Inventory
{
    public interface IInventoryDystuffService
    {
        Tuple<List<InventoryDystuffViewModel>, int> GetReport(string storageCode, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(string storageCode, DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
