//using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentExpenditureWasteProductionsConfig
{
    public class GarmentExpenditureWasteProductionsConfig : IEntityTypeConfiguration<GarmentExpenditureWasteProductions>
    {
        public void Configure(EntityTypeBuilder<GarmentExpenditureWasteProductions> builder)
        {
            builder.Property(p => p.GarmentExpenditureWasteNo)
                .HasMaxLength(25)
                .IsRequired();

            builder.HasIndex(i => i.GarmentExpenditureWasteNo)
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");

            builder.Property(p => p.ExpenditureTo).HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(3000);
            builder.Property(p => p.BCOutNo).HasMaxLength(25);
            builder.Property(p => p.BCOutType).HasMaxLength(25);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentExpenditureWasteProductions)
                .HasForeignKey(f => f.GarmentExpenditureWasteId)
                .IsRequired();
        }
    }
}
