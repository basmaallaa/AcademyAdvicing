using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeManageByIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coordinates_ManageById",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "prerequisite",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ManageById",
                table: "Courses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coordinates_ManageById",
                table: "Courses",
                column: "ManageById",
                principalTable: "Coordinates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coordinates_ManageById",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "prerequisite",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManageById",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coordinates_ManageById",
                table: "Courses",
                column: "ManageById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
