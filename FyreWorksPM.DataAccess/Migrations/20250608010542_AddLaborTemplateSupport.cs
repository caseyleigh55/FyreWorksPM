using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLaborTemplateSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LaborTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LaborRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegularDirectRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RegularBilledRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvernightDirectRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvernightBilledRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LaborTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaborTemplateModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborRates_LaborTemplates_LaborTemplateModelId",
                        column: x => x.LaborTemplateModelId,
                        principalTable: "LaborTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Normal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Lift = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Panel = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pipe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LaborTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaborTemplateModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationHours_LaborTemplates_LaborTemplateModelId",
                        column: x => x.LaborTemplateModelId,
                        principalTable: "LaborTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaborRates_LaborTemplateModelId",
                table: "LaborRates",
                column: "LaborTemplateModelId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationHours_LaborTemplateModelId",
                table: "LocationHours",
                column: "LaborTemplateModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaborRates");

            migrationBuilder.DropTable(
                name: "LocationHours");

            migrationBuilder.DropTable(
                name: "LaborTemplates");
        }
    }
}
