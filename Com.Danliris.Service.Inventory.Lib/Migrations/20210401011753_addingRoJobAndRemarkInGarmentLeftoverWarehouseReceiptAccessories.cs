using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class addingRoJobAndRemarkInGarmentLeftoverWarehouseReceiptAccessories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ROJob",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ROJob",
                table: "GarmentLeftoverWarehouseReceiptFabricItems");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "GarmentLeftoverWarehouseReceiptFabricItems");
        }
    }
}
