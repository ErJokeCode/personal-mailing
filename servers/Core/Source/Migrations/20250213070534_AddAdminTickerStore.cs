using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminTickerStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthenticationTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<byte[]>(type: "bytea", nullable: false),
                    Expires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationTickets_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTickets_AdminId",
                table: "AuthenticationTickets",
                column: "AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationTickets");
        }
    }
}
