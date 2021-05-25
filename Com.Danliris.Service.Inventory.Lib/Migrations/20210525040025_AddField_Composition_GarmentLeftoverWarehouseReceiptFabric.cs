using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_Composition_GarmentLeftoverWarehouseReceiptFabric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Composition",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                maxLength: 3000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Composition",
                table: "GarmentLeftoverWarehouseReceiptFabricItems");
        }
    }
}
