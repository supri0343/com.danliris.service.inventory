using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureFinishedGood>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureFinishedGood> builder)
        {
            builder.Property(p => p.FinishedGoodExpenditureNo)
                .HasMaxLength(25)
                .IsRequired();

            builder.HasIndex(i => i.FinishedGoodExpenditureNo)
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");

            builder.Property(p => p.ExpenditureTo).HasMaxLength(50);
            builder.Property(p => p.OtherDescription).HasMaxLength(100);

            builder.Property(p => p.BuyerCode).HasMaxLength(20);
            builder.Property(p => p.BuyerName).HasMaxLength(100);

            builder.Property(p => p.Description).HasMaxLength(4000);

            builder.Property(p => p.LocalSalesNoteNo).HasMaxLength(50);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentLeftoverWarehouseExpenditureFinishedGood)
                .HasForeignKey(f => f.FinishedGoodExpenditureId)
                .IsRequired();
        }
    }
}
