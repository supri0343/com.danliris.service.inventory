using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class move_ExpenditureGoodNo_GLWReceiptFinishedGoodItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "BuyerCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "BuyerName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "ComodityCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "ComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "ComodityName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "ExpenditureGoodId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "ExpenditureGoodNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.DropColumn(
                name: "RONo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods");

            migrationBuilder.AlterColumn<string>(
                name: "UnitFromName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExpenditureDesc",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BuyerId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComodityCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ComodityName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenditureGoodId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ExpenditureGoodNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RONo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "BuyerCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "BuyerName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "ComodityCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "ComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "ComodityName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "ExpenditureGoodId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "ExpenditureGoodNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropColumn(
                name: "RONo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.AlterColumn<string>(
                name: "UnitFromName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExpenditureDesc",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BuyerId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComodityCode",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ComodityId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ComodityName",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenditureGoodId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ExpenditureGoodNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RONo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UomUnit",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
