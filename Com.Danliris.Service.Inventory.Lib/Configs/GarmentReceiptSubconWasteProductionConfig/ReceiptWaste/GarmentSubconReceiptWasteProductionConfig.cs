using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentReceiptSubconWasteProduction.ReceiptWaste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.GarmentReceiptSubconWasteProductionConfig.ReceiptWaste
{
    public class GarmentSubconReceiptWasteProductionConfig : IEntityTypeConfiguration<GarmentSubconReceiptWasteProductions>
    {
        public void Configure(EntityTypeBuilder<GarmentSubconReceiptWasteProductions> builder)
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
               .WithOne(w => w.GarmentSubconReceiptWasteProductions)
               .HasForeignKey(f => f.GarmentReceiptWasteId)
               .IsRequired();
        }
    }
}
