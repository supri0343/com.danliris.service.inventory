using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class addingGarmentLeftoverWarehouseReceiptAccessories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseReceiptAccessories",
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
                    InvoiceNoReceive = table.Column<string>(nullable: true),
                    RequestUnitCode = table.Column<string>(nullable: true),
                    RequestUnitName = table.Column<string>(nullable: true),
                    RequestUnitId = table.Column<long>(nullable: false),
                    UENNo = table.Column<string>(nullable: true),
                    UENid = table.Column<long>(nullable: false),
                    StorageFromCode = table.Column<string>(nullable: true),
                    StorageFromId = table.Column<long>(nullable: false),
                    StorageFromName = table.Column<string>(nullable: true),
                    ExpenditureDate = table.Column<DateTimeOffset>(nullable: false),
                    StorageReceiveDate = table.Column<DateTimeOffset>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseReceiptAccessories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseReceiptAccessoryItems",
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
                    POSerialNumber = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    ProductCode = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductRemark = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    UomUnitId = table.Column<long>(nullable: false),
                    UomUnit = table.Column<string>(nullable: true),
                    ROJob = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    GarmentLeftOverWarehouseReceiptAccessoriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseReceiptAccessoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentLeftoverWarehouseReceiptAccessoryItems_GarmentLeftoverWarehouseReceiptAccessories_GarmentLeftOverWarehouseReceiptAcce~",
                        column: x => x.GarmentLeftOverWarehouseReceiptAccessoriesId,
                        principalTable: "GarmentLeftoverWarehouseReceiptAccessories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseReceiptAccessoryItems_GarmentLeftOverWarehouseReceiptAccessoriesId",
                table: "GarmentLeftoverWarehouseReceiptAccessoryItems",
                column: "GarmentLeftOverWarehouseReceiptAccessoriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseReceiptAccessoryItems");

            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseReceiptAccessories");
        }
    }
}
