using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Migrations
{
    /// <inheritdoc />
    public partial class deleteGenerateID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Coordinator_GenerateById",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_GenerateById",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "GenerateById",
                table: "Reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerateById",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GenerateById",
                table: "Reports",
                column: "GenerateById");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Coordinator_GenerateById",
                table: "Reports",
                column: "GenerateById",
                principalTable: "Coordinator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
