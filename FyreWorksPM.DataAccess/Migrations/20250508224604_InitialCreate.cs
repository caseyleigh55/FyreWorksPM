using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BidModel_Clients_ClientId",
                table: "BidModel");

            migrationBuilder.DropForeignKey(
                name: "FK_BidModel_SiteInfoModel_SiteInfoId",
                table: "BidModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SiteInfoModel",
                table: "SiteInfoModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BidModel",
                table: "BidModel");

            migrationBuilder.RenameTable(
                name: "SiteInfoModel",
                newName: "BidSiteInfo");

            migrationBuilder.RenameTable(
                name: "BidModel",
                newName: "BidInfo");

            migrationBuilder.RenameIndex(
                name: "IX_BidModel_SiteInfoId",
                table: "BidInfo",
                newName: "IX_BidInfo_SiteInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_BidModel_ClientId",
                table: "BidInfo",
                newName: "IX_BidInfo_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BidSiteInfo",
                table: "BidSiteInfo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BidInfo",
                table: "BidInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BidInfo_BidSiteInfo_SiteInfoId",
                table: "BidInfo",
                column: "SiteInfoId",
                principalTable: "BidSiteInfo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BidInfo_Clients_ClientId",
                table: "BidInfo",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BidInfo_BidSiteInfo_SiteInfoId",
                table: "BidInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_BidInfo_Clients_ClientId",
                table: "BidInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BidSiteInfo",
                table: "BidSiteInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BidInfo",
                table: "BidInfo");

            migrationBuilder.RenameTable(
                name: "BidSiteInfo",
                newName: "SiteInfoModel");

            migrationBuilder.RenameTable(
                name: "BidInfo",
                newName: "BidModel");

            migrationBuilder.RenameIndex(
                name: "IX_BidInfo_SiteInfoId",
                table: "BidModel",
                newName: "IX_BidModel_SiteInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_BidInfo_ClientId",
                table: "BidModel",
                newName: "IX_BidModel_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SiteInfoModel",
                table: "SiteInfoModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BidModel",
                table: "BidModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BidModel_Clients_ClientId",
                table: "BidModel",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BidModel_SiteInfoModel_SiteInfoId",
                table: "BidModel",
                column: "SiteInfoId",
                principalTable: "SiteInfoModel",
                principalColumn: "Id");
        }
    }
}
