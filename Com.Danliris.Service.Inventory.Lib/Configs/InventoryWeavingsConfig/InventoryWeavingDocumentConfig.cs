using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.InventoryWeavingsConfig
{
    public class InventoryWeavingDocumentConfig : IEntityTypeConfiguration<InventoryWeavingDocument>
    {
        public void Configure(EntityTypeBuilder<InventoryWeavingDocument> builder)
        {
            builder.Property(p => p.BonNo).HasMaxLength(255);
            builder.Property(p => p.BonType).HasMaxLength(255);
            builder.Property(p => p.StorageId).HasMaxLength(255);
            builder.Property(p => p.StorageCode).HasMaxLength(255);
            builder.Property(p => p.StorageName).HasMaxLength(255);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.InventoryWeavingDocument)
                .HasForeignKey(f => f.InventoryWeavingDocumentId)
                .IsRequired();
        }
    }
}
