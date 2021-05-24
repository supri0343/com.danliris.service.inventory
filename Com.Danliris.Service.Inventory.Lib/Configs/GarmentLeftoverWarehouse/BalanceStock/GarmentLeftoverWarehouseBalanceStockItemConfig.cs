using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStockItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseBalanceStockItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseBalanceStockItem> builder)
        {
            builder.Property(p => p.UnitCode).HasMaxLength(25);
            builder.Property(p => p.UnitName).HasMaxLength(100);
            builder.Property(p => p.PONo).HasMaxLength(25);
            builder.Property(p => p.UomUnit).HasMaxLength(100);
            builder.Property(p => p.ProductCode).HasMaxLength(25);
            builder.Property(p => p.ProductName).HasMaxLength(100);
            builder.Property(p => p.RONo).HasMaxLength(50);
            builder.Property(p => p.LeftoverComodityName).HasMaxLength(255);
            builder.Property(p => p.LeftoverComodityCode).HasMaxLength(20);
            builder.Property(p => p.ProductRemark).HasMaxLength(3000);
            builder.Property(p => p.Composition).HasMaxLength(255);
            builder.Property(p => p.Construction).HasMaxLength(255);
            builder.Property(p => p.Yarn).HasMaxLength(500);
            builder.Property(p => p.Width).HasMaxLength(500);
        }
    }
}
