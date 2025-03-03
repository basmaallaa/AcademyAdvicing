using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManageById",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UploadedById",
                table: "ScheduleTimeTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GenerateById",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UploadedById",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssignedById",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManageById",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ManageById",
                table: "Students",
                column: "ManageById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTimeTable_UploadedById",
                table: "ScheduleTimeTable",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GenerateById",
                table: "Reports",
                column: "GenerateById");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_UploadedById",
                table: "Materials",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AssignedById",
                table: "Doctors",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ManageById",
                table: "Courses",
                column: "ManageById");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coordinates_ManageById",
                table: "Courses",
                column: "ManageById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Coordinates_AssignedById",
                table: "Doctors",
                column: "AssignedById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Doctors_UploadedById",
                table: "Materials",
                column: "UploadedById",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Coordinates_GenerateById",
                table: "Reports",
                column: "GenerateById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimeTable_Coordinates_UploadedById",
                table: "ScheduleTimeTable",
                column: "UploadedById",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students",
                column: "ManageById",
                principalTable: "StudentAffairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coordinates_ManageById",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Coordinates_AssignedById",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Doctors_UploadedById",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Coordinates_GenerateById",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimeTable_Coordinates_UploadedById",
                table: "ScheduleTimeTable");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAffairs_ManageById",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ManageById",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTimeTable_UploadedById",
                table: "ScheduleTimeTable");

            migrationBuilder.DropIndex(
                name: "IX_Reports_GenerateById",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Materials_UploadedById",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_AssignedById",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ManageById",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ManageById",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                table: "ScheduleTimeTable");

            migrationBuilder.DropColumn(
                name: "GenerateById",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ManageById",
                table: "Courses");
        }
    }
}
