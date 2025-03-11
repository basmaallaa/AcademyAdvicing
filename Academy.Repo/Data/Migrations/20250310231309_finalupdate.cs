using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class finalupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Availablecourses_AvailableCourseId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AvailableCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AvailableCourseId",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Availablecourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Availablecourses_CourseId",
                table: "Availablecourses",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availablecourses_Courses_CourseId",
                table: "Availablecourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availablecourses_Courses_CourseId",
                table: "Availablecourses");

            migrationBuilder.DropIndex(
                name: "IX_Availablecourses_CourseId",
                table: "Availablecourses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Availablecourses");

            migrationBuilder.AddColumn<int>(
                name: "AvailableCourseId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AvailableCourseId",
                table: "Courses",
                column: "AvailableCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Availablecourses_AvailableCourseId",
                table: "Courses",
                column: "AvailableCourseId",
                principalTable: "Availablecourses",
                principalColumn: "Id");
        }
    }
}
