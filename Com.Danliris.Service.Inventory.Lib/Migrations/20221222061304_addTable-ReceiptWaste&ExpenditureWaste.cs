using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class addTableReceiptWasteExpenditureWaste : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentExpenditureWasteProductions",
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
                    GarmentExpenditureWasteNo = table.Column<string>(maxLength: 25, nullable: false),
                    ExpenditureDate = table.Column<DateTimeOffset>(nullable: false),
                    ExpenditureTo = table.Column<string>(maxLength: 50, nullable: true),
                    WasteType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 3000, nullable: true),
                    BCOutNo = table.Column<string>(maxLength: 25, nullable: true),
                    BCOutType = table.Column<string>(maxLength: 25, nullable: true),
                    BCOutDate = table.Column<DateTimeOffset>(nullable: true),
                    ActualQuantity = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentExpenditureWasteProductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentReceiptWasteProductions",
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
                    GarmentReceiptWasteNo = table.Column<string>(maxLength: 25, nullable: false),
                    SourceName = table.Column<string>(maxLength: 25, nullable: true),
                    DestinationName = table.Column<string>(maxLength: 25, nullable: true),
                    ReceiptDate = table.Column<DateTimeOffset>(nullable: false),
                    WasteType = table.Column<string>(maxLength: 25, nullable: true),
                    Remark = table.Column<string>(maxLength: 3000, nullable: true),
                    TotalAval = table.Column<double>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentReceiptWasteProductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentExpenditureWasteProductionItems",
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
                    GarmentExpenditureWasteId = table.Column<int>(nullable: false),
                    GarmentReceiptWasteNo = table.Column<string>(maxLength: 25, nullable: true),
                    GarmentReceiptWasteId = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                   
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentExpenditureWasteProductionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentExpenditureWasteProductionItems_GarmentExpenditureWasteProductions_GarmentExpenditureWasteId",
                        column: x => x.GarmentExpenditureWasteId,
                        principalTable: "GarmentExpenditureWasteProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GarmentReceiptWasteProductionItems",
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
                    GarmentReceiptWasteId = table.Column<int>(nullable: false),
                    BCNo = table.Column<string>(maxLength: 25, nullable: true),
                    BCType = table.Column<string>(maxLength: 25, nullable: true),
                    BCDate = table.Column<DateTimeOffset>(nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    ProductCode = table.Column<string>(maxLength: 25, nullable: true),
                    ProductName = table.Column<string>(maxLength: 100, nullable: true),
                    ProductRemark = table.Column<string>(maxLength: 3000, nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    RONo = table.Column<string>(maxLength: 25, nullable: true),
                    Article = table.Column<string>(maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentReceiptWasteProductionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentReceiptWasteProductionItems_GarmentReceiptWasteProductions_GarmentReceiptWasteId",
                        column: x => x.GarmentReceiptWasteId,
                        principalTable: "GarmentReceiptWasteProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentExpenditureWasteProductionItems_GarmentExpenditureWasteId",
                table: "GarmentExpenditureWasteProductionItems",
                column: "GarmentExpenditureWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentExpenditureWasteProductions_GarmentExpenditureWasteNo",
                table: "GarmentExpenditureWasteProductions",
                column: "GarmentExpenditureWasteNo",
                unique: true,
                filter: "[_IsDeleted]=(0)");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentReceiptWasteProductionItems_GarmentReceiptWasteId",
                table: "GarmentReceiptWasteProductionItems",
                column: "GarmentReceiptWasteId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentReceiptWasteProductions_GarmentReceiptWasteNo",
                table: "GarmentReceiptWasteProductions",
                column: "GarmentReceiptWasteNo",
                unique: true,
                filter: "[_IsDeleted]=(0)");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "GarmentExpenditureWasteProductionItems");

            migrationBuilder.DropTable(
                name: "GarmentReceiptWasteProductionItems");

            migrationBuilder.DropTable(
                name: "GarmentExpenditureWasteProductions");

            migrationBuilder.DropTable(
                name: "GarmentReceiptWasteProductions");
        }
    }
}
