using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class FpReturFromBuyer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationLog");

            migrationBuilder.CreateTable(
                name: "FpReturnFromBuyerModel",
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
                    BuyerId = table.Column<int>(nullable: false),
                    BuyerCode = table.Column<string>(maxLength: 255, nullable: true),
                    BuyerName = table.Column<string>(maxLength: 255, nullable: true),
                    CodeProduct = table.Column<string>(maxLength: 255, nullable: true),
                    CoverLetter = table.Column<string>(maxLength: 255, nullable: true),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    Destination = table.Column<string>(maxLength: 255, nullable: true),
                    SpkNo = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<int>(nullable: false),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FpReturnFromBuyerModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FpReturnFromBuyerDetailModel",
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
                    ProductionOrderId = table.Column<int>(nullable: false),
                    ProductionOrderNo = table.Column<string>(maxLength: 255, nullable: true),
                    FpReturnFromBuyerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FpReturnFromBuyerDetailModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FpReturnFromBuyerDetailModel_FpReturnFromBuyerModel_FpReturnFromBuyerId",
                        column: x => x.FpReturnFromBuyerId,
                        principalTable: "FpReturnFromBuyerModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FpReturnFromBuyerItemModel",
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
                    ColorWay = table.Column<string>(maxLength: 255, nullable: true),
                    DesignCode = table.Column<string>(maxLength: 255, nullable: true),
                    DesignNumber = table.Column<string>(maxLength: 255, nullable: true),
                    Length = table.Column<double>(nullable: false),
                    ProductCode = table.Column<string>(maxLength: 255, nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(maxLength: 255, nullable: true),
                    Remark = table.Column<string>(maxLength: 4000, nullable: true),
                    ReturnQuantity = table.Column<double>(nullable: false),
                    UOM = table.Column<string>(maxLength: 255, nullable: true),
                    UOMId = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    FpReturnFromBuyerDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FpReturnFromBuyerItemModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FpReturnFromBuyerItemModel_FpReturnFromBuyerDetailModel_FpReturnFromBuyerDetailId",
                        column: x => x.FpReturnFromBuyerDetailId,
                        principalTable: "FpReturnFromBuyerDetailModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FpReturnFromBuyerDetailModel_FpReturnFromBuyerId",
                table: "FpReturnFromBuyerDetailModel",
                column: "FpReturnFromBuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_FpReturnFromBuyerItemModel_FpReturnFromBuyerDetailId",
                table: "FpReturnFromBuyerItemModel",
                column: "FpReturnFromBuyerDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FpReturnFromBuyerItemModel");

            migrationBuilder.DropTable(
                name: "FpReturnFromBuyerDetailModel");

            migrationBuilder.DropTable(
                name: "FpReturnFromBuyerModel");

            migrationBuilder.CreateTable(
                name: "IntegrationLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    LastIntegrationDate = table.Column<DateTimeOffset>(nullable: false),
                    Remark = table.Column<string>(maxLength: 100, nullable: true),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationLog", x => x.Id);
                });
        }
    }
}
