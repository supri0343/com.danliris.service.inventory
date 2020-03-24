using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodConfigs
{
    class GarmentLeftoverWarehouseReceiptFinishedGoodItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseReceiptFinishedGoodItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseReceiptFinishedGoodItem> builder)
        {
            builder.Property(p => p.SizeName).HasMaxLength(50);
            builder.Property(p => p.UomUnit).HasMaxLength(255);
            builder.Property(p => p.Remark).HasMaxLength(4000);
        }
    }
}
