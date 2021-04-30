using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class GarmentLeftoverWarehouseExpenditureFabricItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureFabricItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureFabricItem> builder)
        {
            builder.Property(p => p.UnitCode).HasMaxLength(25);
            builder.Property(p => p.UnitName).HasMaxLength(100);
            builder.Property(p => p.PONo).HasMaxLength(25);
            builder.Property(p => p.UomUnit).HasMaxLength(100);

            builder.HasIndex(p => new { p.ExpenditureId, p.StockId })
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");
        }
    }
}
