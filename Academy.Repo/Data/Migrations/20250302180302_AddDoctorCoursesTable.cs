using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorCoursesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorCourses_Courses_CourseId",
                table: "DoctorCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorCourses_Doctors_DoctorId",
                table: "DoctorCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorCourses",
                table: "DoctorCourses");

            migrationBuilder.RenameTable(
                name: "DoctorCourses",
                newName: "doctorCourses");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorCourses_DoctorId",
                table: "doctorCourses",
                newName: "IX_doctorCourses_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_doctorCourses",
                table: "doctorCourses",
                columns: new[] { "CourseId", "DoctorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_doctorCourses_Courses_CourseId",
                table: "doctorCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorCourses_Doctors_DoctorId",
                table: "doctorCourses",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctorCourses_Courses_CourseId",
                table: "doctorCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorCourses_Doctors_DoctorId",
                table: "doctorCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_doctorCourses",
                table: "doctorCourses");

            migrationBuilder.RenameTable(
                name: "doctorCourses",
                newName: "DoctorCourses");

            migrationBuilder.RenameIndex(
                name: "IX_doctorCourses_DoctorId",
                table: "DoctorCourses",
                newName: "IX_DoctorCourses_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorCourses",
                table: "DoctorCourses",
                columns: new[] { "CourseId", "DoctorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorCourses_Courses_CourseId",
                table: "DoctorCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorCourses_Doctors_DoctorId",
                table: "DoctorCourses",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }
    }
}
