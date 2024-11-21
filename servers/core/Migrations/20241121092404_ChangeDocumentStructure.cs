using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDocumentStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "StudentIds",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "Messages");

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

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId",
                table: "Documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId",
                table: "ActiveStudents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_MessageId",
                table: "Documents",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_NotificationId",
                table: "Documents",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_NotificationTemplateId",
                table: "Documents",
                column: "NotificationTemplateId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_NotificationTemplates_NotificationTemplateId",
                table: "Documents",
                column: "NotificationTemplateId",
                principalTable: "NotificationTemplates",
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
                name: "FK_ActiveStudents_NotificationTemplates_NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Messages_MessageId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_NotificationTemplates_NotificationTemplateId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Notifications_NotificationId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_MessageId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_NotificationId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_NotificationTemplateId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_ActiveStudents_NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "NotificationId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId",
                table: "ActiveStudents");

            migrationBuilder.AddColumn<List<int>>(
                name: "DocumentIds",
                table: "NotificationTemplates",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "StudentIds",
                table: "NotificationTemplates",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "DocumentIds",
                table: "Notifications",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "DocumentIds",
                table: "Messages",
                type: "integer[]",
                nullable: true);
        }
    }
}
