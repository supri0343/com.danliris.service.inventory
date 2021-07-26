using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class Add_IsUsed_glw_expenditure_fab_acc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "GarmentLeftoverWarehouseExpenditureFabrics",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "GarmentLeftoverWarehouseExpenditureAccessories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "GarmentLeftoverWarehouseExpenditureFabrics");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "GarmentLeftoverWarehouseExpenditureAccessories");
        }
    }
}
