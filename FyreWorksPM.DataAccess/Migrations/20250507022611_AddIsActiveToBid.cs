using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToBid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Bids",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SiteInfoId",
                table: "Bids",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SiteInfoModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScopeOfWork = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParcelNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jurisdiction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingArea = table.Column<double>(type: "float", nullable: false),
                    NumberOfStories = table.Column<int>(type: "int", nullable: false),
                    OccupancyGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccupantLoad = table.Column<int>(type: "int", nullable: false),
                    ConstructionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSprinklered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteInfoModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bids_SiteInfoId",
                table: "Bids",
                column: "SiteInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_SiteInfoModel_SiteInfoId",
                table: "Bids",
                column: "SiteInfoId",
                principalTable: "SiteInfoModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_SiteInfoModel_SiteInfoId",
                table: "Bids");

            migrationBuilder.DropTable(
                name: "SiteInfoModel");

            migrationBuilder.DropIndex(
                name: "IX_Bids_SiteInfoId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "SiteInfoId",
                table: "Bids");
        }
    }
}
