using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureAccessoriesItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureAccessoriesItem> builder)
        {
            builder.Property(p => p.UnitCode).HasMaxLength(25);
            builder.Property(p => p.UnitName).HasMaxLength(100);
            builder.Property(p => p.PONo).HasMaxLength(25);
            builder.Property(p => p.UomUnit).HasMaxLength(100);
            builder.Property(p => p.ProductCode).HasMaxLength(50);
            builder.Property(p => p.ProductName).HasMaxLength(100);


            builder.HasIndex(p => new { p.ExpenditureId, p.StockId })
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");
        }
    }
}
