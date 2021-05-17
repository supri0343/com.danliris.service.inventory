using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class update_GarmentLeftoverWarehouseBalanceStockItems_Remove_StockId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockId",
                table: "GarmentLeftoverWarehouseBalanceStocksItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "GarmentLeftoverWarehouseBalanceStocksItems",
                nullable: false,
                defaultValue: 0);
        }
    }
}
