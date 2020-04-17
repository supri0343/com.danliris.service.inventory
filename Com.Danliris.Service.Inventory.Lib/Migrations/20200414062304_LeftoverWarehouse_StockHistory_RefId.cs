using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class LeftoverWarehouse_StockHistory_RefId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockReferenceId",
                table: "GarmentLeftoverWarehouseStockHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockReferenceItemId",
                table: "GarmentLeftoverWarehouseStockHistories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockReferenceId",
                table: "GarmentLeftoverWarehouseStockHistories");

            migrationBuilder.DropColumn(
                name: "StockReferenceItemId",
                table: "GarmentLeftoverWarehouseStockHistories");
        }
    }
}
