using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "NotificationSchedules",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSchedules_AdminId",
                table: "NotificationSchedules",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSchedules_AspNetUsers_AdminId",
                table: "NotificationSchedules",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSchedules_AspNetUsers_AdminId",
                table: "NotificationSchedules");

            migrationBuilder.DropIndex(
                name: "IX_NotificationSchedules_AdminId",
                table: "NotificationSchedules");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "NotificationSchedules");
        }
    }
}
