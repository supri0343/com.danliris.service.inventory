using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.LogHistories
{
    public interface ILogHistoryService
    {
        void CreateAsync(string division, string activity);
    }
}
