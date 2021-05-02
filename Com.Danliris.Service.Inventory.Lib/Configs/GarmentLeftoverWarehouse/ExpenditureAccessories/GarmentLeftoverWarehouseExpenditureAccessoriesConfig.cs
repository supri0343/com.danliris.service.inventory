using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseExpenditureAccessories>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseExpenditureAccessories> builder)
        {
            builder.Property(p => p.ExpenditureNo)
                .HasMaxLength(25)
                .IsRequired();

            builder.HasIndex(i => i.ExpenditureNo)
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");

            builder.Property(p => p.UnitExpenditureCode).HasMaxLength(25);
            builder.Property(p => p.UnitExpenditureName).HasMaxLength(100);

            builder.Property(p => p.BuyerCode).HasMaxLength(25);
            builder.Property(p => p.BuyerName).HasMaxLength(100);

            builder.Property(p => p.LocalSalesNoteNo).HasMaxLength(50);

            builder.Property(p => p.EtcRemark).HasMaxLength(500);

            builder.Property(p => p.Remark).HasMaxLength(3000);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentLeftoverWarehouseExpenditureAccessories)
                .HasForeignKey(f => f.ExpenditureId)
                .IsRequired();
        }
    }
}
