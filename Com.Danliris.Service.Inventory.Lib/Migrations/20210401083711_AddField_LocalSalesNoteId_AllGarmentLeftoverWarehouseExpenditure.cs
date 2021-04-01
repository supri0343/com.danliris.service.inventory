using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_LocalSalesNoteId_AllGarmentLeftoverWarehouseExpenditure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureFabrics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureAvals",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods");

            migrationBuilder.DropColumn(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureFabrics");

            migrationBuilder.DropColumn(
                name: "LocalSalesNoteId",
                table: "GarmentLeftoverWarehouseExpenditureAvals");
        }
    }
}
