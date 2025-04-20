using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFMGoSki.Migrations
{
    /// <inheritdoc />
    public partial class FixReservationTimeRangeClassAndCityClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ReservationTimeRange");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDateOnly",
                table: "ReservationTimeRange",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTimeOnly",
                table: "ReservationTimeRange",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDateOnly",
                table: "ReservationTimeRange",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTimeOnly",
                table: "ReservationTimeRange",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Class_CityId",
                table: "Class",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_City_CityId",
                table: "Class",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_City_CityId",
                table: "Class");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropIndex(
                name: "IX_Class_CityId",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "EndDateOnly",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "EndTimeOnly",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "StartDateOnly",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "StartTimeOnly",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Class");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "ReservationTimeRange",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ReservationTimeRange",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
