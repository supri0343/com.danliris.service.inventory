using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_LocalSalesNoteNo_AllGarmentLeftoverWarehouseExpenditure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureFabrics",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureAvals",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods");

            migrationBuilder.DropColumn(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureFabrics");

            migrationBuilder.DropColumn(
                name: "LocalSalesNoteNo",
                table: "GarmentLeftoverWarehouseExpenditureAvals");
        }
    }
}
