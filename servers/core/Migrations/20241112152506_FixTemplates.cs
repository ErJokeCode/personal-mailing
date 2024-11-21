using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class FixTemplates : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
