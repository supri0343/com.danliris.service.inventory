using Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.FpReturnFromBuyerConfigs
{
    public class FpReturnFromBuyerConfig : IEntityTypeConfiguration<FpReturnFromBuyerModel>
    {
        public void Configure(EntityTypeBuilder<FpReturnFromBuyerModel> builder)
        {
            builder
                .HasMany(h => h.Details)
                .WithOne(w => w.FpReturnFromBuyer)
                .HasForeignKey(f => f.FpReturnFromBuyerId)
                .IsRequired();
        }
    }
}
