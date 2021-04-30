using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class remove_uom_ExpenditureFinishedGoodItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UomId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UomId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                nullable: true);
        }
    }
}
