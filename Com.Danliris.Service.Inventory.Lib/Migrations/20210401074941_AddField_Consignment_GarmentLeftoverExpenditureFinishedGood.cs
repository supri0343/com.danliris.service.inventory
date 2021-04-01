using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_Consignment_GarmentLeftoverExpenditureFinishedGood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Consignment",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consignment",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods");
        }
    }
}
