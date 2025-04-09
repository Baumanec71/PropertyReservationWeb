using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreateReviws1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorsViewingStatus",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DeleteStatus",
                table: "PaymentRentalRequests");

            migrationBuilder.RenameColumn(
                name: "RecipientsViewingStatus",
                table: "Reviews",
                newName: "IsCalculated");

            migrationBuilder.AlterColumn<decimal>(
                name: "BonusPoints",
                table: "Users",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "BonusTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsCalculated = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewId = table.Column<long>(type: "bigint", nullable: true),
                    AdvertisementId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonusTransactions_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonusTransactions_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonusTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BonusPoints", "DateOfRegistration" },
                values: new object[] { 100000m, new DateTime(2025, 4, 7, 17, 5, 27, 500, DateTimeKind.Utc).AddTicks(9193) });

            migrationBuilder.CreateIndex(
                name: "IX_BonusTransactions_AdvertisementId",
                table: "BonusTransactions",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusTransactions_ReviewId",
                table: "BonusTransactions",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusTransactions_UserId",
                table: "BonusTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusTransactions");

            migrationBuilder.RenameColumn(
                name: "IsCalculated",
                table: "Reviews",
                newName: "RecipientsViewingStatus");

            migrationBuilder.AlterColumn<int>(
                name: "BonusPoints",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<bool>(
                name: "AuthorsViewingStatus",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteStatus",
                table: "PaymentRentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BonusPoints", "DateOfRegistration" },
                values: new object[] { 100000, new DateTime(2025, 3, 31, 15, 35, 36, 253, DateTimeKind.Utc).AddTicks(9196) });
        }
    }
}
