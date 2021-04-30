using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class RenameTableMovement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryWeavingMovement",
                table: "InventoryWeavingMovement");

            migrationBuilder.RenameTable(
                name: "InventoryWeavingMovement",
                newName: "InventoryWeavingMovements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryWeavingMovements",
                table: "InventoryWeavingMovements",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryWeavingMovements",
                table: "InventoryWeavingMovements");

            migrationBuilder.RenameTable(
                name: "InventoryWeavingMovements",
                newName: "InventoryWeavingMovement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryWeavingMovement",
                table: "InventoryWeavingMovement",
                column: "Id");
        }
    }
}
