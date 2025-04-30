using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Migrations
{
    /// <inheritdoc />
    public partial class addAdmissionYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdmissionYear",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionYear",
                table: "Students");
        }
    }
}
