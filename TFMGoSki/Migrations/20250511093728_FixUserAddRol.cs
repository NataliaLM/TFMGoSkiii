using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFMGoSki.Migrations
{
    /// <inheritdoc />
    public partial class FixUserAddRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassReservation_User_ClientId",
                table: "ClassReservation");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "ClassReservation",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassReservation_ClientId",
                table: "ClassReservation",
                newName: "IX_ClassReservation_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReservation_User_UserId",
                table: "ClassReservation",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassReservation_User_UserId",
                table: "ClassReservation");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ClassReservation",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassReservation_UserId",
                table: "ClassReservation",
                newName: "IX_ClassReservation_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "User",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReservation_User_ClientId",
                table: "ClassReservation",
                column: "ClientId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
