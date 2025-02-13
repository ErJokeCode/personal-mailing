using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameAuthTicketTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationTickets_AspNetUsers_AdminId",
                table: "AuthenticationTickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthenticationTickets",
                table: "AuthenticationTickets");

            migrationBuilder.RenameTable(
                name: "AuthenticationTickets",
                newName: "AdminAuthTickets");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationTickets_AdminId",
                table: "AdminAuthTickets",
                newName: "IX_AdminAuthTickets_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdminAuthTickets",
                table: "AdminAuthTickets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminAuthTickets_AspNetUsers_AdminId",
                table: "AdminAuthTickets",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminAuthTickets_AspNetUsers_AdminId",
                table: "AdminAuthTickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdminAuthTickets",
                table: "AdminAuthTickets");

            migrationBuilder.RenameTable(
                name: "AdminAuthTickets",
                newName: "AuthenticationTickets");

            migrationBuilder.RenameIndex(
                name: "IX_AdminAuthTickets_AdminId",
                table: "AuthenticationTickets",
                newName: "IX_AuthenticationTickets_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthenticationTickets",
                table: "AuthenticationTickets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationTickets_AspNetUsers_AdminId",
                table: "AuthenticationTickets",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
