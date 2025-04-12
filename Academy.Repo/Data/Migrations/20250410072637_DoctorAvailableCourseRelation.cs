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
                name: "doctorAvailableCourses");

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
                name: "doctorAvailableCourses",
                columns: table => new
                {
                    AvailableCourseId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctorAvailableCourses", x => new { x.AvailableCourseId, x.DoctorId });
                    table.ForeignKey(
                        name: "FK_doctorAvailableCourses_Availablecourses_AvailableCourseId",
                        column: x => x.AvailableCourseId,
                        principalTable: "Availablecourses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_doctorAvailableCourses_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctorAvailableCourses_DoctorId",
                table: "doctorAvailableCourses",
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
