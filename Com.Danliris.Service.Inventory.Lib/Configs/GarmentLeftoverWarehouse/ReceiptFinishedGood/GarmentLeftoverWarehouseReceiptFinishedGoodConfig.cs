using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodConfigs
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseReceiptFinishedGood>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseReceiptFinishedGood> builder)
        {
            builder.Property(p => p.FinishedGoodReceiptNo)
                .HasMaxLength(25)
                .IsRequired();

            builder.HasIndex(i => i.FinishedGoodReceiptNo)
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");

            builder.Property(p => p.UnitFromCode).HasMaxLength(25);
            builder.Property(p => p.UnitFromName).HasMaxLength(50);
            builder.Property(p => p.ContractNo).HasMaxLength(20);

            builder.Property(p => p.ExpenditureDesc).HasMaxLength(3000);

            builder.Property(p => p.Description).HasMaxLength(3000);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentLeftoverWarehouseReceiptFinishedGood)
                .HasForeignKey(f => f.FinishedGoodReceiptId)
                .IsRequired();
        }
    }
}
