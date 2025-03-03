using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "UploadedById",
                table: "FinalExamTimeTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FinalExamTimeTable_UploadedById",
                table: "FinalExamTimeTable",
                column: "UploadedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalExamTimeTable_Coordinates_UploadedById",
                table: "FinalExamTimeTable",
                column: "UploadedById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalExamTimeTable_Coordinates_UploadedById",
                table: "FinalExamTimeTable");

            migrationBuilder.DropIndex(
                name: "IX_FinalExamTimeTable_UploadedById",
                table: "FinalExamTimeTable");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                table: "FinalExamTimeTable");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
