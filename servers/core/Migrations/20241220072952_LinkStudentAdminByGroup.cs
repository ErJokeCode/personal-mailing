using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class LinkStudentAdminByGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStudents_AspNetUsers_AdminId",
                table: "ActiveStudents");

            migrationBuilder.DropIndex(
                name: "IX_ActiveStudents_AdminId",
                table: "ActiveStudents");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "ActiveStudents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "ActiveStudents",
                type: "text",
                nullable: true);

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
    }
}
