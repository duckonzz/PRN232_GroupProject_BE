using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenderHealthCare.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestSlots_HealthTests_HealthTestId",
                table: "TestSlots");

            migrationBuilder.DropIndex(
                name: "IX_TestSlots_HealthTestId",
                table: "TestSlots");

            migrationBuilder.AlterColumn<string>(
                name: "HealthTestId",
                table: "TestSlots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "HealthTestScheduleId",
                table: "TestSlots",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlots_HealthTestScheduleId",
                table: "TestSlots",
                column: "HealthTestScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestSlots_HealthTestSchedules_HealthTestScheduleId",
                table: "TestSlots",
                column: "HealthTestScheduleId",
                principalTable: "HealthTestSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestSlots_HealthTestSchedules_HealthTestScheduleId",
                table: "TestSlots");

            migrationBuilder.DropIndex(
                name: "IX_TestSlots_HealthTestScheduleId",
                table: "TestSlots");

            migrationBuilder.DropColumn(
                name: "HealthTestScheduleId",
                table: "TestSlots");

            migrationBuilder.AlterColumn<string>(
                name: "HealthTestId",
                table: "TestSlots",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlots_HealthTestId",
                table: "TestSlots",
                column: "HealthTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestSlots_HealthTests_HealthTestId",
                table: "TestSlots",
                column: "HealthTestId",
                principalTable: "HealthTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
