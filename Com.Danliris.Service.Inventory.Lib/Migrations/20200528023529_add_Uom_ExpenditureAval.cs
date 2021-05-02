using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_Uom_ExpenditureAval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UomId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UomId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.DropColumn(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");
        }
    }
}
