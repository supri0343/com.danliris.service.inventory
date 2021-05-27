using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_BasicPrice_GarmentLeftoverWarehouseReceiptFabricItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentLeftoverWarehouseReceiptFabricItems");
        }
    }
}
