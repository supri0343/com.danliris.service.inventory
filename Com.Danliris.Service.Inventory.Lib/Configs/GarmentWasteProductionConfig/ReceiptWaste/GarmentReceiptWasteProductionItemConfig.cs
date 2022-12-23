using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.ReceiptWaste
{
    public class GarmentReceiptWasteProductionItemConfig : IEntityTypeConfiguration<GarmentReceiptWasteProductionItems>
    {
        public void Configure(EntityTypeBuilder<GarmentReceiptWasteProductionItems> builder)
        {
            builder.Property(p => p.BCNo).HasMaxLength(25);
            builder.Property(p => p.BCType).HasMaxLength(25);

            builder.Property(p => p.ProductCode).HasMaxLength(25);
            builder.Property(p => p.ProductName).HasMaxLength(100);
            builder.Property(p => p.ProductRemark).HasMaxLength(3000);

            builder.Property(p => p.RONo).HasMaxLength(25);
            builder.Property(p => p.Article).HasMaxLength(3000);
        }
    
    }
}
