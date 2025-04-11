using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Notify.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Notify.Migrations
{
    /// <inheritdoc />
    public partial class ChangeErrorsToStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationError");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:NotificationState", "lost,read,sent");

            migrationBuilder.CreateTable(
                name: "NotificationStatus",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<NotificationState>(type: "\"NotificationState\"", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationStatus", x => new { x.NotificationId, x.Id });
                    table.ForeignKey(
                        name: "FK_NotificationStatus_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationStatus");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:NotificationState", "lost,read,sent");

            migrationBuilder.CreateTable(
                name: "NotificationError",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationError", x => new { x.NotificationId, x.Id });
                    table.ForeignKey(
                        name: "FK_NotificationError_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
