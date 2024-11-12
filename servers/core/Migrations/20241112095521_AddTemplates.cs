using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudentNotification_Notifications_NotificationsId",
                table: "ActiveStudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_AdminId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationStatuses_Notifications_NotificationId",
                table: "NotificationStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_AdminId",
                table: "Notification",
                newName: "IX_Notification_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudentNotification_Notification_NotificationsId",
                table: "ActiveStudentNotification",
                column: "NotificationsId",
                principalTable: "Notification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Notification_NotificationId",
                table: "Documents",
                column: "NotificationId",
                principalTable: "Notification",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_AdminId",
                table: "Notification",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationStatuses_Notification_NotificationId",
                table: "NotificationStatuses",
                column: "NotificationId",
                principalTable: "Notification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudentNotification_Notification_NotificationsId",
                table: "ActiveStudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Notification_NotificationId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_AdminId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationStatuses_Notification_NotificationId",
                table: "NotificationStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_AdminId",
                table: "Notifications",
                newName: "IX_Notifications_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudentNotification_Notifications_NotificationsId",
                table: "ActiveStudentNotification",
                column: "NotificationsId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_AdminId",
                table: "Notifications",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationStatuses_Notifications_NotificationId",
                table: "NotificationStatuses",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
