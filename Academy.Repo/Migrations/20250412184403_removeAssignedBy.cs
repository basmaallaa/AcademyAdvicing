using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Migrations
{
    /// <inheritdoc />
    public partial class removeAssignedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Coordinates_AssignedById",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_AssignedById",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Doctors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedById",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AssignedById",
                table: "Doctors",
                column: "AssignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Coordinates_AssignedById",
                table: "Doctors",
                column: "AssignedById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
