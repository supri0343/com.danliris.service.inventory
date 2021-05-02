using Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Configs.FpReturnFromBuyerConfigs
{
    public class FpReturnFromBuyerDetailConfig : IEntityTypeConfiguration<FpReturnFromBuyerDetailModel>
    {
        public void Configure(EntityTypeBuilder<FpReturnFromBuyerDetailModel> builder)
        {
            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.FpReturnFromBuyerDetail)
                .HasForeignKey(f => f.FpReturnFromBuyerDetailId)
                .IsRequired();
        }
    }
}
