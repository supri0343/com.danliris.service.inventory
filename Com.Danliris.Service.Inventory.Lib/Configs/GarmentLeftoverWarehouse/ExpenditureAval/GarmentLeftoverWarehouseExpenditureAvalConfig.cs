using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAvalConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureAval>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureAval> builder)
        {
            builder.Property(p => p.AvalExpenditureNo)
                .HasMaxLength(25)
                .IsRequired();

            builder.HasIndex(i => i.AvalExpenditureNo)
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");

            builder.Property(p => p.ExpenditureTo).HasMaxLength(50);
            builder.Property(p => p.OtherDescription).HasMaxLength(100);

            builder.Property(p => p.BuyerCode).HasMaxLength(25);
            builder.Property(p => p.BuyerName).HasMaxLength(100);

            builder.Property(p => p.Description).HasMaxLength(3000);

            builder.Property(p => p.LocalSalesNoteNo)
                .HasMaxLength(50);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentLeftoverWarehouseExpenditureAval)
                .HasForeignKey(f => f.AvalExpenditureId)
                .IsRequired();
        }
    }
}
