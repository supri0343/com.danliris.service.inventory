using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.InventoryWeavingsConfig
{
    public class InventoryWeavingMovementConfig : IEntityTypeConfiguration<InventoryWeavingMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryWeavingMovement> builder)
        {
            builder.Property(p => p.ProductOrderName).HasMaxLength(255);
            builder.Property(p => p.ReferenceNo).HasMaxLength(255);
            builder.Property(p => p.Construction).HasMaxLength(255);
            //builder.Property(p => p.StorageId).HasMaxLength(255);
            //builder.Property(p => p.StorageCode).HasMaxLength(255);
            //builder.Property(p => p.StorageName).HasMaxLength(255);
            builder.Property(p => p.Grade).HasMaxLength(255);
            builder.Property(p => p.Piece).HasMaxLength(1000);
            //builder.Property(p => p.ProductName).HasMaxLength(4000);
            builder.Property(p => p.UomUnit).HasMaxLength(255);
            builder.Property(p => p.UomId).HasMaxLength(255);
        }
    }
}
