using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddField_QtyKG_GarmentLeftoverWarehouseExpenditureFabricItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "QtyKG",
                table: "GarmentLeftoverWarehouseExpenditureFabrics",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtyKG",
                table: "GarmentLeftoverWarehouseExpenditureFabrics");
        }
    }
}
