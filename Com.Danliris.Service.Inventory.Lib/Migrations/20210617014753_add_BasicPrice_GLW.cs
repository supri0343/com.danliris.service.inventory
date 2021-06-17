using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_BasicPrice_GLW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseStocks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseReceiptAccessoryItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseExpenditureFabricItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseBalanceStocksItems",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseStocks");

            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseReceiptAccessoryItems");

            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseExpenditureFabricItems");

            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems");

            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseBalanceStocksItems");
        }
    }
}
