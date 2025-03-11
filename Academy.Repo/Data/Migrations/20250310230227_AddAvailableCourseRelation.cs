using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableCourseRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "AcademicYear",
                table: "Availablecourses",
                newName: "AcademicYears");

            migrationBuilder.AlterColumn<int>(
                name: "ManageById",
                table: "Students",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students",
                column: "ManageById",
                principalTable: "StudentAffairs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Availablecourses_AvailableCourseId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AvailableCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AvailableCourseId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "AcademicYears",
                table: "Availablecourses",
                newName: "AcademicYear");

            migrationBuilder.AlterColumn<int>(
                name: "ManageById",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students",
                column: "ManageById",
                principalTable: "StudentAffairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
