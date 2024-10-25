using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AdaptNewParser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Students_StudentId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Notifications",
                newName: "ActiveStudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_StudentId",
                table: "Notifications",
                newName: "IX_Notifications_ActiveStudentId");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActiveStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<string>(type: "text", nullable: true),
                    PersonalNumber = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    Patronymic = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TypeOfCost = table.Column<string>(type: "text", nullable: true),
                    TypeOfEducation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveStudents", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ActiveStudents_ActiveStudentId",
                table: "Notifications",
                column: "ActiveStudentId",
                principalTable: "ActiveStudents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ActiveStudents_ActiveStudentId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "ActiveStudents");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ActiveStudentId",
                table: "Notifications",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ActiveStudentId",
                table: "Notifications",
                newName: "IX_Notifications_StudentId");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PersonalNumber = table.Column<string>(type: "text", nullable: true),
                    UserCourseId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Students_StudentId",
                table: "Notifications",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
