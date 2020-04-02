using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock
{
    public interface IGarmentLeftoverWarehouseStockService
    {
        Task<int> StockIn(GarmentLeftoverWarehouseStock stock, string StockReferenceNo);
        Task<int> StockOut(GarmentLeftoverWarehouseStock stock, string StockReferenceNo);
    }
}
