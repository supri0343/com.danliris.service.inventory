using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalConfigs
{
    public class GarmentLeftoverWarehouseReceiptAvalItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseReceiptAvalItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseReceiptAvalItem> builder)
        {
            builder.Property(p => p.RONo).HasMaxLength(25);

            builder.Property(p => p.ProductCode).HasMaxLength(25);
            builder.Property(p => p.ProductName).HasMaxLength(100);
            builder.Property(p => p.ProductRemark).HasMaxLength(3000);

            builder.Property(p => p.BCNo).HasMaxLength(25);
            builder.Property(p => p.BCType).HasMaxLength(25);
            builder.Property(p => p.POSerialNumber).HasMaxLength(25);

            builder.Property(p => p.UomUnit).HasMaxLength(100);

        }
    }
}
