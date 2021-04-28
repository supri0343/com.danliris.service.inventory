using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.InventoryWeavingsConfig
{
    public class InventoryWeavingDocumentItemConfig : IEntityTypeConfiguration<InventoryWeavingDocumentItem>
    {
        public void Configure(EntityTypeBuilder<InventoryWeavingDocumentItem> builder)
        {
            builder.Property(p => p.ProductOrderName).HasMaxLength(255);
            builder.Property(p => p.ReferenceNo).HasMaxLength(255);
            builder.Property(p => p.Construction).HasMaxLength(255);
            builder.Property(p => p.Grade).HasMaxLength(255);
            builder.Property(p => p.UomUnit).HasMaxLength(255);
            builder.Property(p => p.UomId).HasMaxLength(255);
        }
    }
}
