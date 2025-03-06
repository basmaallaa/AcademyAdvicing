using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class newInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students");

            migrationBuilder.AlterColumn<int>(
                name: "ManageById",
                table: "Students",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students");

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
