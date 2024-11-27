using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplatesToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudents_NotificationTemplates_NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.DropIndex(
                name: "IX_ActiveStudents_NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.CreateTable(
                name: "ActiveStudentNotificationTemplate",
                columns: table => new
                {
                    ActiveStudentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplatesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveStudentNotificationTemplate", x => new { x.ActiveStudentsId, x.TemplatesId });
                    table.ForeignKey(
                        name: "FK_ActiveStudentNotificationTemplate_ActiveStudents_ActiveStud~",
                        column: x => x.ActiveStudentsId,
                        principalTable: "ActiveStudents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveStudentNotificationTemplate_NotificationTemplates_Tem~",
                        column: x => x.TemplatesId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveStudentNotificationTemplate_TemplatesId",
                table: "ActiveStudentNotificationTemplate",
                column: "TemplatesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveStudentNotificationTemplate");

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId",
                table: "ActiveStudents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActiveStudents_NotificationTemplateId",
                table: "ActiveStudents",
                column: "NotificationTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudents_NotificationTemplates_NotificationTemplateId",
                table: "ActiveStudents",
                column: "NotificationTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id");
        }
    }
}
