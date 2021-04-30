using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_LeftoverComodity_GarmentLeftoverWarehouseExpenditureFinishedGoodItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeftoverComodityCode",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LeftoverComodityId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "LeftoverComodityName",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftoverComodityCode",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityName",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");
        }
    }
}
