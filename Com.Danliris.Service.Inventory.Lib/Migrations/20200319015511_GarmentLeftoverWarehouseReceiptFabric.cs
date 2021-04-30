using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class GarmentLeftoverWarehouseReceiptFabric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseReceiptFabrics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    ReceiptNoteNo = table.Column<string>(maxLength: 25, nullable: false),
                    UnitFromId = table.Column<long>(nullable: false),
                    UnitFromCode = table.Column<string>(maxLength: 25, nullable: true),
                    UnitFromName = table.Column<string>(maxLength: 100, nullable: true),
                    UENId = table.Column<long>(nullable: false),
                    UENNo = table.Column<string>(maxLength: 25, nullable: true),
                    StorageFromId = table.Column<long>(nullable: false),
                    StorageFromCode = table.Column<string>(maxLength: 25, nullable: true),
                    StorageFromName = table.Column<string>(maxLength: 100, nullable: true),
                    ExpenditureDate = table.Column<DateTimeOffset>(nullable: false),
                    ReceiptDate = table.Column<DateTimeOffset>(nullable: false),
                    Remark = table.Column<string>(maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseReceiptFabrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseReceiptFabricItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    GarmentLeftoverWarehouseReceiptFabricId = table.Column<int>(nullable: false),
                    UENItemId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    ProductCode = table.Column<string>(maxLength: 25, nullable: true),
                    ProductName = table.Column<string>(maxLength: 100, nullable: true),
                    ProductRemark = table.Column<string>(maxLength: 3000, nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    UomId = table.Column<long>(nullable: false),
                    UomUnit = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseReceiptFabricItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentLeftoverWarehouseReceiptFabricItems_GarmentLeftoverWarehouseReceiptFabrics_GarmentLeftoverWarehouseReceiptFabricId",
                        column: x => x.GarmentLeftoverWarehouseReceiptFabricId,
                        principalTable: "GarmentLeftoverWarehouseReceiptFabrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseReceiptFabricItems_GarmentLeftoverWarehouseReceiptFabricId",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                column: "GarmentLeftoverWarehouseReceiptFabricId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseReceiptFabrics_ReceiptNoteNo",
                table: "GarmentLeftoverWarehouseReceiptFabrics",
                column: "ReceiptNoteNo",
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseReceiptFabricItems");

            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseReceiptFabrics");
        }
    }
}
