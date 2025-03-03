using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class CourseStudentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Assignedcourses",
                table: "Assignedcourses");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Courses",
                newName: "CourseId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Assignedcourses",
                newName: "CourseId");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "Assignedcourses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Assignedcourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId1",
                table: "Assignedcourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assignedcourses",
                table: "Assignedcourses",
                columns: new[] { "StudentId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignedcourses_CourseId1",
                table: "Assignedcourses",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_Assignedcourses_StudentId1",
                table: "Assignedcourses",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignedcourses_Courses_CourseId1",
                table: "Assignedcourses",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignedcourses_Students_StudentId1",
                table: "Assignedcourses",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignedcourses_Courses_CourseId1",
                table: "Assignedcourses");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignedcourses_Students_StudentId1",
                table: "Assignedcourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assignedcourses",
                table: "Assignedcourses");

            migrationBuilder.DropIndex(
                name: "IX_Assignedcourses_CourseId1",
                table: "Assignedcourses");

            migrationBuilder.DropIndex(
                name: "IX_Assignedcourses_StudentId1",
                table: "Assignedcourses");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Assignedcourses");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Assignedcourses");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "Assignedcourses");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Courses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Assignedcourses",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assignedcourses",
                table: "Assignedcourses",
                column: "Id");
        }
    }
}
