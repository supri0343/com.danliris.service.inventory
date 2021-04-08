using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_LocalSalesNoteNo_GarmentLeftoverWarehouseExpenditureAccessories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureAccessories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureAccessories",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureAccessories");

            migrationBuilder.DropColumn(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureAccessories");
        }
    }
}
