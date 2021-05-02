using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodConfigs
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseReceiptFinishedGoodItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseReceiptFinishedGoodItem> builder)
        {
            builder.Property(p => p.ExpenditureGoodNo).HasMaxLength(25);
            builder.Property(p => p.SizeName).HasMaxLength(50);
            builder.Property(p => p.UomUnit).HasMaxLength(20);
            builder.Property(p => p.Remark).HasMaxLength(4000);
            builder.Property(p => p.RONo).HasMaxLength(20);
            builder.Property(p => p.BuyerCode).HasMaxLength(20);
            builder.Property(p => p.BuyerName).HasMaxLength(255);
            builder.Property(p => p.LeftoverComodityName).HasMaxLength(255);
            builder.Property(p => p.LeftoverComodityCode).HasMaxLength(20);
            builder.Property(p => p.ComodityName).HasMaxLength(255);
            builder.Property(p => p.ComodityCode).HasMaxLength(20);
        }
    }
}
