using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balans",
                table: "Users",
                newName: "BonusPoints");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Advertisements",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateChange = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdUserAdmin = table.Column<long>(type: "bigint", nullable: false),
                    IdAdvertisement = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Advertisements_IdAdvertisement",
                        column: x => x.IdAdvertisement,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Users_IdUserAdmin",
                        column: x => x.IdUserAdmin,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Balance", "BonusPoints", "DateOfRegistration" },
                values: new object[] { 0m, 100000, new DateTime(2025, 3, 23, 14, 38, 19, 945, DateTimeKind.Utc).AddTicks(6244) });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_IdAdvertisement",
                table: "ApprovalRequests",
                column: "IdAdvertisement");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_IdUserAdmin",
                table: "ApprovalRequests",
                column: "IdUserAdmin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "BonusPoints",
                table: "Users",
                newName: "Balans");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Advertisements",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Balans", "DateOfRegistration" },
                values: new object[] { 0, new DateTime(2025, 3, 4, 16, 58, 18, 422, DateTimeKind.Utc).AddTicks(1676) });
        }
    }
}
