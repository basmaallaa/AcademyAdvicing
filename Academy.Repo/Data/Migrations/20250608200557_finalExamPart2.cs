using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class finalExamPart2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_FinalExamTimeTable_finalExamTimeTableId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_finalExamTimeTableId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "finalExamTimeTableId",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_FinalExamTimeTable_ScheduleId",
                table: "Students",
                column: "ScheduleId",
                principalTable: "FinalExamTimeTable",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_FinalExamTimeTable_ScheduleId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "finalExamTimeTableId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_finalExamTimeTableId",
                table: "Students",
                column: "finalExamTimeTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_FinalExamTimeTable_finalExamTimeTableId",
                table: "Students",
                column: "finalExamTimeTableId",
                principalTable: "FinalExamTimeTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
