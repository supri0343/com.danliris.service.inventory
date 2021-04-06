using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_LeftoverComodity_LeftoverWarehouseStocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.AddColumn<string>(
                name: "LeftoverComodityCode",
                table: "GarmentLeftoverWarehouseStocks",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LeftoverComodityId",
                table: "GarmentLeftoverWarehouseStocks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeftoverComodityName",
                table: "GarmentLeftoverWarehouseStocks",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_ProductId_UomId_LeftoverComodityId",
                table: "GarmentLeftoverWarehouseStocks",
                columns: new[] { "ReferenceType", "UnitId", "PONo", "RONo", "ProductId", "UomId", "LeftoverComodityId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_ProductId_UomId_LeftoverComodityId",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityCode",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityId",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityName",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseStocks_ReferenceType_UnitId_PONo_RONo_ProductId_UomId",
                table: "GarmentLeftoverWarehouseStocks",
                columns: new[] { "ReferenceType", "UnitId", "PONo", "RONo", "ProductId", "UomId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }
    }
}
