using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class Initial_ExpenditureAval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseExpenditureAvals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    AvalExpenditureNo = table.Column<string>(maxLength: 25, nullable: false),
                    ExpenditureDate = table.Column<DateTimeOffset>(nullable: false),
                    ExpenditureTo = table.Column<string>(maxLength: 50, nullable: true),
                    AvalType = table.Column<string>(nullable: true),
                    BuyerId = table.Column<long>(nullable: false),
                    BuyerCode = table.Column<string>(maxLength: 25, nullable: true),
                    BuyerName = table.Column<string>(maxLength: 100, nullable: true),
                    OtherDescription = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseExpenditureAvals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseExpenditureAvalItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    StockId = table.Column<int>(nullable: false),
                    UnitId = table.Column<long>(nullable: false),
                    UnitCode = table.Column<string>(maxLength: 25, nullable: true),
                    UnitName = table.Column<string>(maxLength: 100, nullable: true),
                    AvalReceiptNo = table.Column<string>(maxLength: 25, nullable: true),
                    AvalReceiptId = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    AvalExpenditureId = table.Column<int>(nullable: false),
                    UomId = table.Column<long>(nullable: false),
                    UomUnit = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseExpenditureAvalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentLeftoverWarehouseExpenditureAvalItems_GarmentLeftoverWarehouseExpenditureAvals_AvalExpenditureId",
                        column: x => x.AvalExpenditureId,
                        principalTable: "GarmentLeftoverWarehouseExpenditureAvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_AvalExpenditureId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                column: "AvalExpenditureId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvalItems_AvalReceiptId_StockId",
                table: "GarmentLeftoverWarehouseExpenditureAvalItems",
                columns: new[] { "AvalReceiptId", "StockId" },
                unique: true,
                filter: "[_IsDeleted]=(0)");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureAvals_AvalExpenditureNo",
                table: "GarmentLeftoverWarehouseExpenditureAvals",
                column: "AvalExpenditureNo",
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseExpenditureAvalItems");

            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseExpenditureAvals");
        }
    }
}
