using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class Initial_GarmentLeftoverWarehouseFinishedGoods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseReceiptFinishedGoods",
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
                    FinishedGoodReceiptNo = table.Column<string>(maxLength: 25, nullable: false),
                    ExpenditureGoodNo = table.Column<string>(maxLength: 25, nullable: true),
                    ExpenditureGoodId = table.Column<Guid>(nullable: false),
                    UnitFromId = table.Column<int>(nullable: false),
                    UnitFromCode = table.Column<string>(maxLength: 25, nullable: true),
                    UnitFromName = table.Column<string>(maxLength: 100, nullable: true),
                    RONo = table.Column<string>(nullable: true),
                    Article = table.Column<string>(nullable: true),
                    BuyerId = table.Column<int>(nullable: false),
                    BuyerCode = table.Column<string>(nullable: true),
                    BuyerName = table.Column<string>(nullable: true),
                    ComodityId = table.Column<int>(nullable: false),
                    ComodityCode = table.Column<string>(nullable: true),
                    ComodityName = table.Column<string>(nullable: true),
                    Invoice = table.Column<string>(nullable: true),
                    ContractNo = table.Column<string>(nullable: true),
                    Carton = table.Column<double>(nullable: false),
                    ExpenditureDesc = table.Column<string>(nullable: true),
                    ExpenditureDate = table.Column<DateTimeOffset>(nullable: false),
                    Description = table.Column<string>(maxLength: 3000, nullable: true),
                    ReceiptDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseReceiptFinishedGoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
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
                    ExpenditureGoodItemId = table.Column<Guid>(nullable: false),
                    SizeId = table.Column<int>(nullable: false),
                    SizeCode = table.Column<string>(maxLength: 50, nullable: true),
                    SizeName = table.Column<string>(maxLength: 50, nullable: true),
                    UomUnit = table.Column<string>(maxLength: 255, nullable: true),
                    UomId = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(maxLength: 4000, nullable: true),
                    FinishedGoodReceiptId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentLeftoverWarehouseReceiptFinishedGoodItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentLeftoverWarehouseReceiptFinishedGoodItems_GarmentLeftoverWarehouseReceiptFinishedGoods_FinishedGoodReceiptId",
                        column: x => x.FinishedGoodReceiptId,
                        principalTable: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseReceiptFinishedGoodItems_FinishedGoodReceiptId",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoodItems",
                column: "FinishedGoodReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentLeftoverWarehouseReceiptFinishedGoods_FinishedGoodReceiptNo",
                table: "GarmentLeftoverWarehouseReceiptFinishedGoods",
                column: "FinishedGoodReceiptNo",
                unique: true,
                filter: "[_IsDeleted]=(0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseReceiptFinishedGoodItems");

            migrationBuilder.DropTable(
                name: "GarmentLeftoverWarehouseReceiptFinishedGoods");
        }
    }
}
