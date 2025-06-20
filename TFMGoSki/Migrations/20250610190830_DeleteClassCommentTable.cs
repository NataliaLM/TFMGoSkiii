using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFMGoSki.Migrations
{
    /// <inheritdoc />
    public partial class DeleteClassCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassComment");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Comment",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassReservationId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ClassReservationId",
                table: "Comment",
                column: "ClassReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_ClassReservation_ClassReservationId",
                table: "Comment",
                column: "ClassReservationId",
                principalTable: "ClassReservation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_ClassReservation_ClassReservationId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ClassReservationId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ClassReservationId",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Comment",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.CreateTable(
                name: "ClassComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ClassReservationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassComment_ClassReservation_ClassReservationId",
                        column: x => x.ClassReservationId,
                        principalTable: "ClassReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassComment_Comment_Id",
                        column: x => x.Id,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassComment_ClassReservationId",
                table: "ClassComment",
                column: "ClassReservationId");
        }
    }
}
