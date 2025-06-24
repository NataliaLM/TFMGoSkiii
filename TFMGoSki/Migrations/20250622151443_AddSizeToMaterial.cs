using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFMGoSki.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeToMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Material",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Material");
        }
    }
}
