using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddingReceiptAvalComponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AvalComponentId",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AvalComponentNo",
                table: "GarmentLeftoverWarehouseReceiptAvalItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");

            migrationBuilder.DropColumn(
                name: "AvalComponentId",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");

            migrationBuilder.DropColumn(
                name: "AvalComponentNo",
                table: "GarmentLeftoverWarehouseReceiptAvalItems");
        }
    }
}
