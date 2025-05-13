using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBidTaskModelAndLinkToTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskName",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tasks",
                newName: "TaskModelId");

            migrationBuilder.CreateTable(
                name: "TaskModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskModelId",
                table: "Tasks",
                column: "TaskModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskModel_TaskModelId",
                table: "Tasks",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskModel_TaskModelId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskModel");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskModelId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TaskModelId",
                table: "Tasks",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "TaskName",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
