using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class update_idx_GLW_ExpenditureAvalItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                column: "StockId",
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }
    }
}
