using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_FabricRemark_GarmentLeftoverWarehouseFabric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FabricRemark",
                table: "GarmentLeftoverWarehouseReceiptFabricItems",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductCode",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FabricRemark",
                table: "GarmentLeftoverWarehouseReceiptFabricItems");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductCode",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25,
                oldNullable: true);
        }
    }
}
