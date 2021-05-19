using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class update_GarmentLeftoverWarehouseExpenditureAccessoriesItems_add_Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "GarmentLeftoverWarehouseExpenditureAccessoriesItems");
        }
    }
}
