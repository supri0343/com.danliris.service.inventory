using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_isUsed_ReceiptAval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UomId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.DropColumn(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "GarmentLeftoverWarehouseReceiptAvals",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "GarmentLeftoverWarehouseReceiptAvals");

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
    }
}
