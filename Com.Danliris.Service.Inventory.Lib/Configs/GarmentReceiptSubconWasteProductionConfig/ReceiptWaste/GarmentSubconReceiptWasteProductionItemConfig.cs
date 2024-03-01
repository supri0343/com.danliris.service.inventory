using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentReceiptSubconWasteProduction.ReceiptWaste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.GarmentReceiptSubconWasteProductionConfig.ReceiptWaste
{
    public class GarmentSubconReceiptWasteProductionItemConfig : IEntityTypeConfiguration<GarmentSubconReceiptWasteProductionItems>
    {
        public void Configure(EntityTypeBuilder<GarmentSubconReceiptWasteProductionItems> builder)
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
