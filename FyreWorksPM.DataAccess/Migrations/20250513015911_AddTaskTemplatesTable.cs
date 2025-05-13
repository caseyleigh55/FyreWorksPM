using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyreWorksPM.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTemplatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskModel_TaskModelId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel");

            migrationBuilder.RenameTable(
                name: "TaskModel",
                newName: "TaskTemplates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskTemplates",
                table: "TaskTemplates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskTemplates_TaskModelId",
                table: "Tasks",
                column: "TaskModelId",
                principalTable: "TaskTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskTemplates_TaskModelId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskTemplates",
                table: "TaskTemplates");

            migrationBuilder.RenameTable(
                name: "TaskTemplates",
                newName: "TaskModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskModel_TaskModelId",
                table: "Tasks",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
