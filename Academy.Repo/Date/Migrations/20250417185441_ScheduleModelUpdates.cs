using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Date.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleModelUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableCourseId",
                table: "ScheduleTimeTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTimeTable_AvailableCourseId",
                table: "ScheduleTimeTable",
                column: "AvailableCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimeTable_Availablecourses_AvailableCourseId",
                table: "ScheduleTimeTable",
                column: "AvailableCourseId",
                principalTable: "Availablecourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimeTable_Availablecourses_AvailableCourseId",
                table: "ScheduleTimeTable");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTimeTable_AvailableCourseId",
                table: "ScheduleTimeTable");

            migrationBuilder.DropColumn(
                name: "AvailableCourseId",
                table: "ScheduleTimeTable");
        }
    }
}
