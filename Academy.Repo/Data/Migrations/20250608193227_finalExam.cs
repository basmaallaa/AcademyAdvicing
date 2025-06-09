using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class finalExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinalId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "finalExamTimeTableId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AvailableCourseId",
                table: "FinalExamTimeTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_finalExamTimeTableId",
                table: "Students",
                column: "finalExamTimeTableId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalExamTimeTable_AvailableCourseId",
                table: "FinalExamTimeTable",
                column: "AvailableCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalExamTimeTable_Availablecourses_AvailableCourseId",
                table: "FinalExamTimeTable",
                column: "AvailableCourseId",
                principalTable: "Availablecourses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_FinalExamTimeTable_finalExamTimeTableId",
                table: "Students",
                column: "finalExamTimeTableId",
                principalTable: "FinalExamTimeTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalExamTimeTable_Availablecourses_AvailableCourseId",
                table: "FinalExamTimeTable");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_FinalExamTimeTable_finalExamTimeTableId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_finalExamTimeTableId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_FinalExamTimeTable_AvailableCourseId",
                table: "FinalExamTimeTable");

            migrationBuilder.DropColumn(
                name: "FinalId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "finalExamTimeTableId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AvailableCourseId",
                table: "FinalExamTimeTable");
        }
    }
}
