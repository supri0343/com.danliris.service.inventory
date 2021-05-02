using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class Update_GarmentLeftoverWarehouseReceiptFinishedGoodItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.AlterColumn<long>(
                name: "UnitFromId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "ComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "BuyerId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "UomId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "SizeId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UnitFromId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "ComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "BuyerId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "UomId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "SizeId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "SizeCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 50,
                nullable: true);
        }
    }
}
