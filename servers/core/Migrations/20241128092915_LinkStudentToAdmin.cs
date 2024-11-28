using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class LinkStudentToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdminChatId",
                table: "ActiveStudents",
                newName: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveStudents_AdminId",
                table: "ActiveStudents",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStudents_AspNetUsers_AdminId",
                table: "ActiveStudents",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudents_AspNetUsers_AdminId",
                table: "ActiveStudents");

            migrationBuilder.DropIndex(
                name: "IX_ActiveStudents_AdminId",
                table: "ActiveStudents");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "ActiveStudents",
                newName: "AdminChatId");
        }
    }
}
