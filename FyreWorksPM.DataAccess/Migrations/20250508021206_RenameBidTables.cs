using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameBidTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Clients_ClientId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_SiteInfoModel_SiteInfoId",
                table: "Bids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bids",
                table: "Bids");

            migrationBuilder.RenameTable(
                name: "Bids",
                newName: "BidModel");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_SiteInfoId",
                table: "BidModel",
                newName: "IX_BidModel_SiteInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_ClientId",
                table: "BidModel",
                newName: "IX_BidModel_ClientId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BidModel_Clients_ClientId",
                table: "BidModel");

            migrationBuilder.DropForeignKey(
                name: "FK_BidModel_SiteInfoModel_SiteInfoId",
                table: "BidModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BidModel",
                table: "BidModel");

            migrationBuilder.RenameTable(
                name: "BidModel",
                newName: "Bids");

            migrationBuilder.RenameIndex(
                name: "IX_BidModel_SiteInfoId",
                table: "Bids",
                newName: "IX_Bids_SiteInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_BidModel_ClientId",
                table: "Bids",
                newName: "IX_Bids_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bids",
                table: "Bids",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Clients_ClientId",
                table: "Bids",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_SiteInfoModel_SiteInfoId",
                table: "Bids",
                column: "SiteInfoId",
                principalTable: "SiteInfoModel",
                principalColumn: "Id");
        }
    }
}
