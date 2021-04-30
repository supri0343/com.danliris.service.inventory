using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class add_Product_ExpenditureAval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");
        }
    }
}
