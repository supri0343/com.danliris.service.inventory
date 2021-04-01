using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class Add_LeftoverComodity_GarmentLeftoverWarehouseReceiptFinishedGoodItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeftoverComodityCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LeftoverComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "LeftoverComodityName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftoverComodityCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "LeftoverComodityName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");
        }
    }
}
