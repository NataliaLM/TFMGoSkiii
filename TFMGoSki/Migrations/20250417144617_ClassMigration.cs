using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFMGoSki.Migrations
{
    /// <inheritdoc />
    public partial class ClassMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassLevel",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Instructor_TP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor_TP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationTimeRange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    RemainingStudentsQuantity = table.Column<int>(type: "int", nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTimeRange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationTimeRange_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Class_InstructorId",
                table: "Class",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTimeRange_ClassId",
                table: "ReservationTimeRange",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Instructor_TP_InstructorId",
                table: "Class",
                column: "InstructorId",
                principalTable: "Instructor_TP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_Instructor_TP_InstructorId",
                table: "Class");

            migrationBuilder.DropTable(
                name: "Instructor_TP");

            migrationBuilder.DropTable(
                name: "ReservationTimeRange");

            migrationBuilder.DropIndex(
                name: "IX_Class_InstructorId",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "ClassLevel",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Class");
        }
    }
}
