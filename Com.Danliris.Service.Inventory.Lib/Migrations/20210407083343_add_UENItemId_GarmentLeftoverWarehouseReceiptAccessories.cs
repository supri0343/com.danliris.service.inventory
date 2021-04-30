using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_UENItemId_GarmentLeftoverWarehouseReceiptAccessories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UENItemId",
                table: "GarmentLeftoverWarehouseReceiptAccessoryItems",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UENItemId",
                table: "GarmentLeftoverWarehouseReceiptAccessoryItems");
        }
    }
}
