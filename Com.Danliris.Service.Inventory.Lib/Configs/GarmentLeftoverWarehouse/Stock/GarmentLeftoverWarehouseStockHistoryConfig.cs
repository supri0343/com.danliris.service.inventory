using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockHistoryConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseStockHistory>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseStockHistory> builder)
        {
            builder.Property(p => p.StockType)
                .HasMaxLength(10)
                .HasConversion<string>();
        }
    }
}
