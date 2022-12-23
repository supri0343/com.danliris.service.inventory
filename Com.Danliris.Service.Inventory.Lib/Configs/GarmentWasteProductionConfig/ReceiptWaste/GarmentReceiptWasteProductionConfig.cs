using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.ReceiptWaste
{
    public class GarmentReceiptWasteProductionConfig : IEntityTypeConfiguration<GarmentReceiptWasteProductions>
    {
        public void Configure(EntityTypeBuilder<GarmentReceiptWasteProductions> builder)
        {
            builder.Property(p => p.GarmentReceiptWasteNo)
              .HasMaxLength(25)
              .IsRequired();

            builder.HasIndex(i => i.GarmentReceiptWasteNo)
                .IsUnique()
                .HasFilter("[_IsDeleted]=(0)");

            builder.Property(p => p.SourceName).HasMaxLength(25);
            builder.Property(p => p.DestinationName).HasMaxLength(25);
            builder.Property(p => p.WasteType).HasMaxLength(25);
            builder.Property(p => p.Remark).HasMaxLength(3000);

            builder
               .HasMany(h => h.Items)
               .WithOne(w => w.GarmentReceiptWasteProductions)
               .HasForeignKey(f => f.GarmentReceiptWasteId)
               .IsRequired();
        }
    }
}
