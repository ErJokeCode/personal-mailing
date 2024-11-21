using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class FixDocumentIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "DocumentIds",
                table: "NotificationTemplates",
                type: "integer[]",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "Messages");
        }
    }
}
