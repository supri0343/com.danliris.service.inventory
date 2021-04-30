using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class LeftoverWarehouse_Stock_Remove_KG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_KG_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.DropColumn(
                name: "KG",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks",
                columns: new[] { "ReferenceType", "UnitId", "PONo", "RONo", "ProductId", "UomId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.AddColumn<double>(
                name: "KG",
                table: "GarmentLeftoverWarehouseStocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_KG_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks",
                columns: new[] { "ReferenceType", "UnitId", "PONo", "RONo", "KG", "ProductId", "UomId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }
    }
}
