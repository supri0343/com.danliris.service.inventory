using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class update_index_GLW_ExpenditureAval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_AvalReceiptId_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                column: "StockId",
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_AvalReceiptId_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                columns: new[] { "AvalReceiptId", "StockId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }
    }
}
