using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class SeparateTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudentNotification_Notification_NotificationsId",
                table: "ActiveStudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Notification_NotificationId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_AdminId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationStatuses_Notification_NotificationId",
                table: "NotificationStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Documents_MessageId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_NotificationId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "NotificationId",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_AdminId",
                table: "Notifications",
                newName: "IX_Notifications_AdminId");

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId",
                table: "ActiveStudents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true),
                    AdminId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTemplates_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveStudents_NotificationTemplateId",
                table: "ActiveStudents",
                column: "NotificationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_AdminId",
                table: "NotificationTemplates",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudentNotification_Notifications_NotificationsId",
                table: "ActiveStudentNotification",
                column: "NotificationsId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudents_NotificationTemplates_NotificationTemplateId",
                table: "ActiveStudents",
                column: "NotificationTemplateId",
                principalTable: "NotificationTemplates",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudentNotification_Notifications_NotificationsId",
                table: "ActiveStudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudents_NotificationTemplates_NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_AdminId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationStatuses_Notifications_NotificationId",
                table: "NotificationStatuses");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ActiveStudents_NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_AdminId",
                table: "Notification",
                newName: "IX_Notification_AdminId");

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "Documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationId",
                table: "Documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_MessageId",
                table: "Documents",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_NotificationId",
                table: "Documents",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudentNotification_Notification_NotificationsId",
                table: "ActiveStudentNotification",
                column: "NotificationsId",
                principalTable: "Notification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

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
    }
}
