﻿// <auto-generated />
using System;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250214135813_AddNotificationDocuments")]
    partial class AddNotificationDocuments
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Identity.AdminAuthTicket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AdminId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Value")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("AdminAuthTickets");
                });

            modelBuilder.Entity("Core.Models.Admin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateOnly>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Core.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("AdminId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Core.Models.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("ChatId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("DeactivatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("NotificationStudent", b =>
                {
                    b.Property<int>("NotificationsId")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uuid");

                    b.HasKey("NotificationsId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("NotificationStudent");
                });

            modelBuilder.Entity("Core.Identity.AdminAuthTicket", b =>
                {
                    b.HasOne("Core.Models.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Core.Models.Notification", b =>
                {
                    b.HasOne("Core.Models.Admin", "Admin")
                        .WithMany("Notifications")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Core.Models.Document", "Documents", b1 =>
                        {
                            b1.Property<int>("NotificationId")
                                .HasColumnType("integer");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("BlobId")
                                .HasColumnType("uuid");

                            b1.Property<string>("MimeType")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("NotificationId", "Id");

                            b1.ToTable("Document");

                            b1.WithOwner()
                                .HasForeignKey("NotificationId");
                        });

                    b.OwnsMany("Core.Models.NotificationError", "Errors", b1 =>
                        {
                            b1.Property<int>("NotificationId")
                                .HasColumnType("integer");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Message")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<Guid>("StudentId")
                                .HasColumnType("uuid");

                            b1.HasKey("NotificationId", "Id");

                            b1.ToTable("NotificationError");

                            b1.WithOwner()
                                .HasForeignKey("NotificationId");
                        });

                    b.Navigation("Admin");

                    b.Navigation("Documents");

                    b.Navigation("Errors");
                });

            modelBuilder.Entity("Core.Models.Student", b =>
                {
                    b.OwnsOne("Core.Abstractions.Parser.ParserStudent", "Info", b1 =>
                        {
                            b1.Property<Guid>("StudentId")
                                .HasColumnType("uuid");

                            b1.Property<string>("DateOfBirth")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Email")
                                .HasColumnType("text");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Patronymic")
                                .HasColumnType("text");

                            b1.Property<string>("PersonalNumber")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<bool?>("Status")
                                .HasColumnType("boolean");

                            b1.Property<string>("Surname")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("TypeOfCost")
                                .HasColumnType("text");

                            b1.Property<string>("TypeOfEducation")
                                .HasColumnType("text");

                            b1.HasKey("StudentId");

                            b1.ToTable("Students");

                            b1.WithOwner()
                                .HasForeignKey("StudentId");

                            b1.OwnsOne("Core.Abstractions.Parser.Group", "Group", b2 =>
                                {
                                    b2.Property<Guid>("ParserStudentStudentId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("DirectionCode")
                                        .HasColumnType("text");

                                    b2.Property<string>("NameSpeciality")
                                        .HasColumnType("text");

                                    b2.Property<string>("Number")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<int>("NumberCourse")
                                        .HasColumnType("integer");

                                    b2.HasKey("ParserStudentStudentId");

                                    b2.ToTable("Students");

                                    b2.WithOwner()
                                        .HasForeignKey("ParserStudentStudentId");
                                });

                            b1.OwnsMany("Core.Abstractions.Parser.CourseInfo", "OnlineCourse", b2 =>
                                {
                                    b2.Property<Guid>("ParserStudentStudentId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b2.Property<int>("Id"));

                                    b2.Property<string>("DateStart")
                                        .HasColumnType("text");

                                    b2.Property<string>("Deadline")
                                        .HasColumnType("text");

                                    b2.Property<string>("Info")
                                        .HasColumnType("text");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Scores")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("University")
                                        .HasColumnType("text");

                                    b2.HasKey("ParserStudentStudentId", "Id");

                                    b2.ToTable("Students_OnlineCourse");

                                    b2.WithOwner()
                                        .HasForeignKey("ParserStudentStudentId");
                                });

                            b1.OwnsMany("Core.Abstractions.Parser.Subject", "Subjects", b2 =>
                                {
                                    b2.Property<Guid>("ParserStudentStudentId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b2.Property<int>("Id"));

                                    b2.Property<string>("FormEducation")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("FullName")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("GroupTgLink")
                                        .HasColumnType("text");

                                    b2.Property<string>("Info")
                                        .HasColumnType("text");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.HasKey("ParserStudentStudentId", "Id");

                                    b2.ToTable("Subject");

                                    b2.WithOwner()
                                        .HasForeignKey("ParserStudentStudentId");

                                    b2.OwnsOne("Core.Abstractions.Parser.CourseInfo", "OnlineCourse", b3 =>
                                        {
                                            b3.Property<Guid>("SubjectParserStudentStudentId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("SubjectId")
                                                .HasColumnType("integer");

                                            b3.Property<string>("DateStart")
                                                .HasColumnType("text");

                                            b3.Property<string>("Deadline")
                                                .HasColumnType("text");

                                            b3.Property<string>("Info")
                                                .HasColumnType("text");

                                            b3.Property<string>("Name")
                                                .IsRequired()
                                                .HasColumnType("text");

                                            b3.Property<string>("Scores")
                                                .IsRequired()
                                                .HasColumnType("text");

                                            b3.Property<string>("University")
                                                .HasColumnType("text");

                                            b3.HasKey("SubjectParserStudentStudentId", "SubjectId");

                                            b3.ToTable("Subject");

                                            b3.WithOwner()
                                                .HasForeignKey("SubjectParserStudentStudentId", "SubjectId");
                                        });

                                    b2.OwnsMany("Core.Abstractions.Parser.Team", "Teams", b3 =>
                                        {
                                            b3.Property<Guid>("SubjectParserStudentStudentId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("SubjectId")
                                                .HasColumnType("integer");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b3.Property<int>("Id"));

                                            b3.Property<string>("Name")
                                                .IsRequired()
                                                .HasColumnType("text");

                                            b3.PrimitiveCollection<string[]>("Teachers")
                                                .IsRequired()
                                                .HasColumnType("text[]");

                                            b3.HasKey("SubjectParserStudentStudentId", "SubjectId", "Id");

                                            b3.ToTable("Team");

                                            b3.WithOwner()
                                                .HasForeignKey("SubjectParserStudentStudentId", "SubjectId");
                                        });

                                    b2.Navigation("OnlineCourse");

                                    b2.Navigation("Teams");
                                });

                            b1.Navigation("Group")
                                .IsRequired();

                            b1.Navigation("OnlineCourse");

                            b1.Navigation("Subjects");
                        });

                    b.Navigation("Info")
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Core.Models.Admin", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Core.Models.Admin", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Models.Admin", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Core.Models.Admin", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NotificationStudent", b =>
                {
                    b.HasOne("Core.Models.Notification", null)
                        .WithMany()
                        .HasForeignKey("NotificationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Models.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Models.Admin", b =>
                {
                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
