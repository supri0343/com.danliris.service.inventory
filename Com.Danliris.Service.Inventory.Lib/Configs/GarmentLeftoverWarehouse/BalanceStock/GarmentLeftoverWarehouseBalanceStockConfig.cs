using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStockConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseBalanceStock>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseBalanceStock> builder)
        {
            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentLeftoverWarehouseBalanceStock)
                .HasForeignKey(f => f.BalanceStockId)
                .IsRequired();
        }
    }
}
