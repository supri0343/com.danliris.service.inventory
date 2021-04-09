using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAvalItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureAvalItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureAvalItem> builder)
        {
            builder.Property(p => p.UnitCode).HasMaxLength(25);
            builder.Property(p => p.UnitName).HasMaxLength(100);
            builder.Property(p => p.AvalReceiptNo).HasMaxLength(25);
            builder.Property(p => p.UomUnit).HasMaxLength(100);
            builder.Property(p => p.ProductCode).HasMaxLength(25);
            builder.Property(p => p.ProductName).HasMaxLength(100);

        }
    }
}
