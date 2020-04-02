using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class GarmentLeftOver_Stock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "POSerialNumber",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseStocks",
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
                    ReferenceType = table.Column<string>(maxLength: 25, nullable: false),
                    UnitId = table.Column<long>(nullable: false),
                    UnitCode = table.Column<string>(maxLength: 25, nullable: true),
                    UnitName = table.Column<string>(maxLength: 100, nullable: true),
                    PONo = table.Column<string>(maxLength: 25, nullable: true),
                    RONo = table.Column<string>(maxLength: 25, nullable: true),
                    KG = table.Column<double>(nullable: true),
                    ProductId = table.Column<long>(nullable: true),
                    ProductCode = table.Column<string>(maxLength: 25, nullable: true),
                    ProductName = table.Column<string>(maxLength: 100, nullable: true),
                    UomId = table.Column<long>(nullable: true),
                    UomUnit = table.Column<string>(maxLength: 100, nullable: true),
                    Quantity = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseStockHistories",
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
                    StockId = table.Column<int>(nullable: false),
                    StockReferenceNo = table.Column<string>(nullable: true),
                    StockType = table.Column<string>(maxLength: 10, nullable: false),
                    BeforeQuantity = table.Column<double>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    AfterQuantity = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseStockHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentLeftoverWarehouseStockHistories_GarmentLeftoverWarehouseStocks_StockId",
                        column: x => x.StockId,
                        principalTable: "GarmentLeftoverWarehouseStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseStockHistories_StockId",
                table: "GarmentLeftoverWarehouseStockHistories",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_KG_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks",
                columns: new[] { "ReferenceType", "UnitId", "PONo", "RONo", "KG", "ProductId", "UomId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseStockHistories");

            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.DropColumn(
                name: "POSerialNumber",
                table: "GarmentLeftoverWarehouseReceiptFabricItems");
        }
    }
}
