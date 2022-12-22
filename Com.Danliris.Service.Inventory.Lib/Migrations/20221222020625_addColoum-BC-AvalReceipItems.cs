using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class addColoumBCAvalReceipItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BCDate",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BCNo",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BCType",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POSerialNumber",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                maxLength: 25,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BCDate",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");

            migrationBuilder.DropColumn(
                name: "BCNo",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");

            migrationBuilder.DropColumn(
                name: "BCType",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");

            migrationBuilder.DropColumn(
                name: "POSerialNumber",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");
        }
    }
}
