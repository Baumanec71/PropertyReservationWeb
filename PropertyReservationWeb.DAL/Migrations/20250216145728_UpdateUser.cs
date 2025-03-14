using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementConveniences");

            migrationBuilder.DropTable(
                name: "Conveniences");

            migrationBuilder.RenameColumn(
                name: "PlacementStatus",
                table: "Advertisements",
                newName: "ConfirmationStatus");

            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "Users",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Balans",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteStatus",
                table: "Photos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Photos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Photos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Photos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Amenityes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenityes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementAmenityes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAdvertisement = table.Column<long>(type: "bigint", nullable: false),
                    IdAmenity = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementAmenityes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementAmenityes_Advertisements_IdAdvertisement",
                        column: x => x.IdAdvertisement,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvertisementAmenityes_Amenityes_IdAmenity",
                        column: x => x.IdAmenity,
                        principalTable: "Amenityes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Avatar", "Balans", "DateOfRegistration" },
                values: new object[] { null, 0, new DateTime(2025, 2, 16, 14, 57, 28, 57, DateTimeKind.Utc).AddTicks(8037) });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementAmenityes_IdAdvertisement",
                table: "AdvertisementAmenityes",
                column: "IdAdvertisement");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementAmenityes_IdAmenity",
                table: "AdvertisementAmenityes",
                column: "IdAmenity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementAmenityes");

            migrationBuilder.DropTable(
                name: "Amenityes");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Balans",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeleteStatus",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "ConfirmationStatus",
                table: "Advertisements",
                newName: "PlacementStatus");

            migrationBuilder.CreateTable(
                name: "Conveniences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conveniences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementConveniences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAdvertisement = table.Column<long>(type: "bigint", nullable: false),
                    IdConvenience = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementConveniences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementConveniences_Advertisements_IdAdvertisement",
                        column: x => x.IdAdvertisement,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvertisementConveniences_Conveniences_IdConvenience",
                        column: x => x.IdConvenience,
                        principalTable: "Conveniences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2024, 12, 9, 13, 59, 24, 751, DateTimeKind.Utc).AddTicks(5223));

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementConveniences_IdAdvertisement",
                table: "AdvertisementConveniences",
                column: "IdAdvertisement");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementConveniences_IdConvenience",
                table: "AdvertisementConveniences",
                column: "IdConvenience");
        }
    }
}
