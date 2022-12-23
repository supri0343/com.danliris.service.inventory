using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.ExpenditureWasteConfig
{
    public class GarmentExpenditureWasteProductionsItemConfig : IEntityTypeConfiguration<GarmentExpenditureWasteProductionItems>
    {
        public void Configure(EntityTypeBuilder<GarmentExpenditureWasteProductionItems> builder)
        {
            builder.Property(p => p.GarmentReceiptWasteNo).HasMaxLength(25);
        }


    }
}
