using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanSlate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BidSiteInfo",
                columns: table => new
                {
                    SiteInfoModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteInfoModelScopeOfWork = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelParcelNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelJurisdiction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelBuildingArea = table.Column<double>(type: "float", nullable: false),
                    SiteInfoModelNumberOfStories = table.Column<int>(type: "int", nullable: false),
                    SiteInfoModelOccupancyGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelOccupantLoad = table.Column<int>(type: "int", nullable: false),
                    SiteInfoModelConstructionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteInfoModelIsSprinklered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidSiteInfo", x => x.SiteInfoModelId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientModelContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientModelEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientModelPhone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientModelId);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    ItemTypeModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeModelName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.ItemTypeModelId);
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                columns: table => new
                {
                    TaskModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskModelTaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskModelType = table.Column<int>(type: "int", nullable: false),
                    TaskModelDefaultCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaskModelDefaultSale = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.TaskModelId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserModelUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModelEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModelPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserModelId);
                });

            migrationBuilder.CreateTable(
                name: "BidInfo",
                columns: table => new
                {
                    BidModelBidId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BidModelBidNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BidModelProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BidModelClientId = table.Column<int>(type: "int", nullable: false),
                    BidModelCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BidModelIsActive = table.Column<bool>(type: "bit", nullable: false),
                    BidModelSiteInfoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidInfo", x => x.BidModelBidId);
                    table.ForeignKey(
                        name: "FK_BidInfo_BidSiteInfo_BidModelSiteInfoId",
                        column: x => x.BidModelSiteInfoId,
                        principalTable: "BidSiteInfo",
                        principalColumn: "SiteInfoModelId");
                    table.ForeignKey(
                        name: "FK_BidInfo_Clients_BidModelClientId",
                        column: x => x.BidModelClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemModelDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemModelItemTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemModelId);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemModelItemTypeId",
                        column: x => x.ItemModelItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "ItemTypeModelId");
                });

            migrationBuilder.CreateTable(
                name: "BidTasks",
                columns: table => new
                {
                    BidTaskModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BidTaskModelBidId = table.Column<int>(type: "int", nullable: false),
                    BidTaskModelTaskModelId = table.Column<int>(type: "int", nullable: false),
                    BidTaskModelCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BidTaskModelSale = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidTasks", x => x.BidTaskModelId);
                    table.ForeignKey(
                        name: "FK_BidTasks_BidInfo_BidTaskModelBidId",
                        column: x => x.BidTaskModelBidId,
                        principalTable: "BidInfo",
                        principalColumn: "BidModelBidId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BidTasks_TaskTemplates_BidTaskModelTaskModelId",
                        column: x => x.BidTaskModelTaskModelId,
                        principalTable: "TaskTemplates",
                        principalColumn: "TaskModelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BidInfo_BidModelClientId",
                table: "BidInfo",
                column: "BidModelClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BidInfo_BidModelSiteInfoId",
                table: "BidInfo",
                column: "BidModelSiteInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_BidTasks_BidTaskModelBidId",
                table: "BidTasks",
                column: "BidTaskModelBidId");

            migrationBuilder.CreateIndex(
                name: "IX_BidTasks_BidTaskModelTaskModelId",
                table: "BidTasks",
                column: "BidTaskModelTaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemModelItemTypeId",
                table: "Items",
                column: "ItemModelItemTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidTasks");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BidInfo");

            migrationBuilder.DropTable(
                name: "TaskTemplates");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "BidSiteInfo");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
