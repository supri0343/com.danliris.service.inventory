using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureFinishedGoodItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureFinishedGoodItem> builder)
        {
            builder.Property(p => p.UnitCode).HasMaxLength(20);
            builder.Property(p => p.UnitName).HasMaxLength(255);
            builder.Property(p => p.RONo).HasMaxLength(50);
            builder.Property(p => p.LeftoverComodityName).HasMaxLength(255);
            builder.Property(p => p.LeftoverComodityCode).HasMaxLength(20);
        }
    }
}
