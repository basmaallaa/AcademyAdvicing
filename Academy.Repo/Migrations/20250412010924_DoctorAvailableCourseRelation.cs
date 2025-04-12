using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Migrations
{
    /// <inheritdoc />
    public partial class DoctorAvailableCourseRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availablecourses_Courses_CourseId",
                table: "Availablecourses");

            migrationBuilder.DropTable(
                name: "doctorCourses");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Availablecourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Availablecourses_DoctorId",
                table: "Availablecourses",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availablecourses_Courses_CourseId",
                table: "Availablecourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Availablecourses_Doctors_DoctorId",
                table: "Availablecourses",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availablecourses_Courses_CourseId",
                table: "Availablecourses");

            migrationBuilder.DropForeignKey(
                name: "FK_Availablecourses_Doctors_DoctorId",
                table: "Availablecourses");

            migrationBuilder.DropIndex(
                name: "IX_Availablecourses_DoctorId",
                table: "Availablecourses");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Availablecourses");

            migrationBuilder.CreateTable(
                name: "doctorCourses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctorCourses", x => new { x.CourseId, x.DoctorId });
                    table.ForeignKey(
                        name: "FK_doctorCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                    table.ForeignKey(
                        name: "FK_doctorCourses_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctorCourses_DoctorId",
                table: "doctorCourses",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availablecourses_Courses_CourseId",
                table: "Availablecourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
