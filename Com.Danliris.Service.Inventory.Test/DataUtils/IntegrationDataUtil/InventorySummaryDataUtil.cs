using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil
{
    public class InventorySummaryDataUtil
    {
        public static InventorySummaryViewModel GetInventorySummary(HttpClientTestService client)
        {
            var response = client.GetAsync($"{APIEndpoint.Inventory}/inventory/inventory-summary?page=1&size=1").Result;
            response.EnsureSuccessStatusCode();

            var data = response.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Result.ToString());

            List<InventorySummaryViewModel> list = JsonConvert.DeserializeObject<List<InventorySummaryViewModel>>(result["data"].ToString());
            InventorySummaryViewModel inventorySummary = list.First();

            return inventorySummary;
        }
    }
}
