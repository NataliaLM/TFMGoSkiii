using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFMGoSki.Migrations
{
    /// <inheritdoc />
    public partial class MaterialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "ReservationTimeRange",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingMaterialsQuantity",
                table: "ReservationTimeRange",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Comment",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<int>(
                name: "ReservationMaterialCartId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterialReservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReservation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuantityMaterial = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false),
                    MaterialStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Material_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_MaterialStatus_MaterialStatusId",
                        column: x => x.MaterialStatusId,
                        principalTable: "MaterialStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_MaterialType_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationMaterialCart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    MaterialReservationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReservationTimeRangeMaterialId = table.Column<int>(type: "int", nullable: false),
                    NumberMaterialsBooked = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationMaterialCart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationMaterialCart_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationMaterialCart_MaterialReservation_MaterialReservationId",
                        column: x => x.MaterialReservationId,
                        principalTable: "MaterialReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationMaterialCart_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationMaterialCart_ReservationTimeRange_ReservationTimeRangeMaterialId",
                        column: x => x.ReservationTimeRangeMaterialId,
                        principalTable: "ReservationTimeRange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTimeRange_MaterialId",
                table: "ReservationTimeRange",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ReservationMaterialCartId",
                table: "Comment",
                column: "ReservationMaterialCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_CityId",
                table: "Material",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_MaterialStatusId",
                table: "Material",
                column: "MaterialStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_MaterialTypeId",
                table: "Material",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReservation_UserId",
                table: "MaterialReservation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationMaterialCart_MaterialId",
                table: "ReservationMaterialCart",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationMaterialCart_MaterialReservationId",
                table: "ReservationMaterialCart",
                column: "MaterialReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationMaterialCart_ReservationTimeRangeMaterialId",
                table: "ReservationMaterialCart",
                column: "ReservationTimeRangeMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationMaterialCart_UserId",
                table: "ReservationMaterialCart",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_ReservationMaterialCart_ReservationMaterialCartId",
                table: "Comment",
                column: "ReservationMaterialCartId",
                principalTable: "ReservationMaterialCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationTimeRange_Material_MaterialId",
                table: "ReservationTimeRange",
                column: "MaterialId",
                principalTable: "Material",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_ReservationMaterialCart_ReservationMaterialCartId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationTimeRange_Material_MaterialId",
                table: "ReservationTimeRange");

            migrationBuilder.DropTable(
                name: "ReservationMaterialCart");

            migrationBuilder.DropTable(
                name: "MaterialReservation");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "MaterialStatus");

            migrationBuilder.DropTable(
                name: "MaterialType");

            migrationBuilder.DropIndex(
                name: "IX_ReservationTimeRange_MaterialId",
                table: "ReservationTimeRange");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ReservationMaterialCartId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "RemainingMaterialsQuantity",
                table: "ReservationTimeRange");

            migrationBuilder.DropColumn(
                name: "ReservationMaterialCartId",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Comment",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
