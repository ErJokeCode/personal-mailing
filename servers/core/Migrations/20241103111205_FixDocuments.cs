using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class FixDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationId",
                table: "Documents",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "Documents",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationId",
                table: "Documents",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "Documents",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
