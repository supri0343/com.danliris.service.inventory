﻿// <auto-generated />
using Com.Danliris.Service.Inventory.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    [DbContext(typeof(InventoryDbContext))]
    [Migration("20180309042923_AddAutoIncrementMaterialDistributionNote")]
    partial class AddAutoIncrementMaterialDistributionNote
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("AutoIncrementNumber");

                    b.Property<bool>("IsApproved");

                    b.Property<bool>("IsDisposition");

                    b.Property<string>("No")
                        .HasMaxLength(255);

                    b.Property<string>("Type")
                        .HasMaxLength(255);

                    b.Property<string>("UnitCode")
                        .HasMaxLength(255);

                    b.Property<string>("UnitId")
                        .HasMaxLength(255);

                    b.Property<string>("UnitName")
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_CreatedUtc");

                    b.Property<string>("_DeletedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_DeletedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_DeletedUtc");

                    b.Property<bool>("_IsDeleted");

                    b.Property<string>("_LastModifiedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_LastModifiedUtc");

                    b.HasKey("Id");

                    b.ToTable("MaterialDistributionNotes");
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNoteDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Grade")
                        .HasMaxLength(255);

                    b.Property<bool>("IsDisposition");

                    b.Property<int>("MaterialDistributionNoteItemId");

                    b.Property<double>("MaterialRequestNoteItemLength");

                    b.Property<int>("MaterialsRequestNoteItemId");

                    b.Property<string>("ProductCode")
                        .HasMaxLength(255);

                    b.Property<string>("ProductId")
                        .HasMaxLength(255);

                    b.Property<string>("ProductName")
                        .HasMaxLength(255);

                    b.Property<string>("ProductionOrderId");

                    b.Property<string>("ProductionOrderNo")
                        .HasMaxLength(255);

                    b.Property<double>("Quantity");

                    b.Property<double>("ReceivedLength");

                    b.Property<string>("SupplierCode")
                        .HasMaxLength(255);

                    b.Property<string>("SupplierId")
                        .HasMaxLength(255);

                    b.Property<string>("SupplierName")
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_CreatedUtc");

                    b.Property<string>("_DeletedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_DeletedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_DeletedUtc");

                    b.Property<bool>("_IsDeleted");

                    b.Property<string>("_LastModifiedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_LastModifiedUtc");

                    b.HasKey("Id");

                    b.HasIndex("MaterialDistributionNoteItemId");

                    b.ToTable("MaterialDistributionNoteDetails");
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNoteItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("MaterialDistributionNoteId");

                    b.Property<string>("MaterialRequestNoteCode")
                        .HasMaxLength(100);

                    b.Property<int>("MaterialRequestNoteId");

                    b.Property<string>("_CreatedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_CreatedUtc");

                    b.Property<string>("_DeletedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_DeletedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_DeletedUtc");

                    b.Property<bool>("_IsDeleted");

                    b.Property<string>("_LastModifiedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_LastModifiedUtc");

                    b.HasKey("Id");

                    b.HasIndex("MaterialDistributionNoteId");

                    b.ToTable("MaterialDistributionNoteItems");
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("AutoIncrementNumber");

                    b.Property<string>("Code")
                        .HasMaxLength(100);

                    b.Property<string>("Remark")
                        .HasMaxLength(1000);

                    b.Property<string>("RequestType")
                        .HasMaxLength(100);

                    b.Property<string>("Type");

                    b.Property<string>("UnitCode")
                        .HasMaxLength(100);

                    b.Property<string>("UnitId")
                        .HasMaxLength(100);

                    b.Property<string>("UnitName")
                        .HasMaxLength(100);

                    b.Property<string>("_CreatedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_CreatedUtc");

                    b.Property<string>("_DeletedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_DeletedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_DeletedUtc");

                    b.Property<bool>("_IsDeleted");

                    b.Property<string>("_LastModifiedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_LastModifiedUtc");

                    b.HasKey("Id");

                    b.ToTable("MaterialsRequestNotes");
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNote_Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Code")
                        .HasMaxLength(100);

                    b.Property<string>("Grade")
                        .HasMaxLength(500);

                    b.Property<double>("Length");

                    b.Property<int>("MaterialsRequestNoteId");

                    b.Property<double>("OrderQuantity");

                    b.Property<string>("OrderTypeCode")
                        .HasMaxLength(100);

                    b.Property<string>("OrderTypeId")
                        .HasMaxLength(100);

                    b.Property<string>("OrderTypeName")
                        .HasMaxLength(100);

                    b.Property<string>("ProductCode")
                        .HasMaxLength(100);

                    b.Property<string>("ProductId")
                        .HasMaxLength(100);

                    b.Property<string>("ProductName")
                        .HasMaxLength(100);

                    b.Property<string>("ProductionOrderId")
                        .HasMaxLength(100);

                    b.Property<string>("ProductionOrderNo")
                        .HasMaxLength(100);

                    b.Property<string>("_CreatedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_CreatedUtc");

                    b.Property<string>("_DeletedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_DeletedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_DeletedUtc");

                    b.Property<bool>("_IsDeleted");

                    b.Property<string>("_LastModifiedAgent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("_LastModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("_LastModifiedUtc");

                    b.HasKey("Id");

                    b.HasIndex("MaterialsRequestNoteId");

                    b.ToTable("MaterialsRequestNote_Items");
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNoteDetail", b =>
                {
                    b.HasOne("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNoteItem", "MaterialDistributionNoteItem")
                        .WithMany("MaterialDistributionNoteDetails")
                        .HasForeignKey("MaterialDistributionNoteItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNoteItem", b =>
                {
                    b.HasOne("Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel.MaterialDistributionNote", "MaterialDistributionNote")
                        .WithMany("MaterialDistributionNoteItems")
                        .HasForeignKey("MaterialDistributionNoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNote_Item", b =>
                {
                    b.HasOne("Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNote", "MaterialsRequestNote")
                        .WithMany("MaterialsRequestNote_Items")
                        .HasForeignKey("MaterialsRequestNoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
