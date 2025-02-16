using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class SerializeParserStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students_OnlineCourse");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropColumn(
                name: "Info_DateOfBirth",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Group_DirectionCode",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Group_NameSpeciality",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Group_Number",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Group_NumberCourse",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Name",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Patronymic",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_PersonalNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_Status",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_TypeOfCost",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Info_TypeOfEducation",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "Info_Surname",
                table: "Students",
                newName: "Info");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Info",
                table: "Students",
                newName: "Info_Surname");

            migrationBuilder.AddColumn<string>(
                name: "Info_DateOfBirth",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Info_Email",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_Group_DirectionCode",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_Group_NameSpeciality",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_Group_Number",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Info_Group_NumberCourse",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Info_Name",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Info_Patronymic",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_PersonalNumber",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Info_Status",
                table: "Students",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_TypeOfCost",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_TypeOfEducation",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Students_OnlineCourse",
                columns: table => new
                {
                    ParserStudentStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateStart = table.Column<string>(type: "text", nullable: true),
                    Deadline = table.Column<string>(type: "text", nullable: true),
                    Info = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Scores = table.Column<string>(type: "text", nullable: false),
                    University = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students_OnlineCourse", x => new { x.ParserStudentStudentId, x.Id });
                    table.ForeignKey(
                        name: "FK_Students_OnlineCourse_Students_ParserStudentStudentId",
                        column: x => x.ParserStudentStudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    ParserStudentStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormEducation = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    GroupTgLink = table.Column<string>(type: "text", nullable: true),
                    Info = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OnlineCourse_DateStart = table.Column<string>(type: "text", nullable: true),
                    OnlineCourse_Deadline = table.Column<string>(type: "text", nullable: true),
                    OnlineCourse_Info = table.Column<string>(type: "text", nullable: true),
                    OnlineCourse_Name = table.Column<string>(type: "text", nullable: true),
                    OnlineCourse_Scores = table.Column<string>(type: "text", nullable: true),
                    OnlineCourse_University = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => new { x.ParserStudentStudentId, x.Id });
                    table.ForeignKey(
                        name: "FK_Subject_Students_ParserStudentStudentId",
                        column: x => x.ParserStudentStudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    SubjectParserStudentStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Teachers = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => new { x.SubjectParserStudentStudentId, x.SubjectId, x.Id });
                    table.ForeignKey(
                        name: "FK_Team_Subject_SubjectParserStudentStudentId_SubjectId",
                        columns: x => new { x.SubjectParserStudentStudentId, x.SubjectId },
                        principalTable: "Subject",
                        principalColumns: new[] { "ParserStudentStudentId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
