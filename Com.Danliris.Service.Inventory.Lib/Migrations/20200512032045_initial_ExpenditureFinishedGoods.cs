using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class initial_ExpenditureFinishedGoods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseExpenditureFinishedGoods",
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
                    FinishedGoodExpenditureNo = table.Column<string>(maxLength: 25, nullable: false),
                    ExpenditureDate = table.Column<DateTimeOffset>(nullable: false),
                    ExpenditureTo = table.Column<string>(maxLength: 50, nullable: true),
                    BuyerId = table.Column<long>(nullable: false),
                    BuyerCode = table.Column<string>(maxLength: 20, nullable: true),
                    BuyerName = table.Column<string>(maxLength: 100, nullable: true),
                    OtherDescription = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseExpenditureFinishedGoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
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
                    UnitId = table.Column<long>(nullable: false),
                    UnitCode = table.Column<string>(maxLength: 20, nullable: true),
                    UnitName = table.Column<string>(maxLength: 255, nullable: true),
                    RONo = table.Column<string>(maxLength: 50, nullable: true),
                    ExpenditureQuantity = table.Column<double>(nullable: false),
                    FinishedGoodExpenditureId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseExpenditureFinishedGoodItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentLeftoverWarehouseExpenditureFinishedGoodItems_GarmentLeftoverWarehouseExpenditureFinishedGoods_FinishedGoodExpenditur~",
                        column: x => x.FinishedGoodExpenditureId,
                        principalTable: "GarmentLeftoverWarehouseExpenditureFinishedGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureFinishedGoodItems_FinishedGoodExpenditureId",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems",
                column: "FinishedGoodExpenditureId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseExpenditureFinishedGoods_FinishedGoodExpenditureNo",
                table: "GarmentLeftoverWarehouseExpenditureFinishedGoods",
                column: "FinishedGoodExpenditureNo",
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseExpenditureFinishedGoodItems");

            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseExpenditureFinishedGoods");
        }
    }
}
