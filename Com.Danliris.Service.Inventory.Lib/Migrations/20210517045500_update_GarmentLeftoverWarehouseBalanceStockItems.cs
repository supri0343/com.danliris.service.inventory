using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class update_GarmentLeftoverWarehouseBalanceStockItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "GarmentLeftoverWarehouseBalanceStocksItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                table: "GarmentLeftoverWarehouseBalanceStocksItems",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Yarn",
                table: "GarmentLeftoverWarehouseBalanceStocksItems",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockId",
                table: "GarmentLeftoverWarehouseBalanceStocksItems");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "GarmentLeftoverWarehouseBalanceStocksItems");

            migrationBuilder.DropColumn(
                name: "Yarn",
                table: "GarmentLeftoverWarehouseBalanceStocksItems");
        }
    }
}
