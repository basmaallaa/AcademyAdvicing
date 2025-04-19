using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class MaterialCourseRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_CourseId",
                table: "Materials",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Courses_CourseId",
                table: "Materials",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Courses_CourseId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_CourseId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Materials");
        }
    }
}
