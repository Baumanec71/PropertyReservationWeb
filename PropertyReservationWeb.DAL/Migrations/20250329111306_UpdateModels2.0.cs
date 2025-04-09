using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorsViewingStatus",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "RecipientsViewingStatus",
                table: "RentalRequests");

            migrationBuilder.AddColumn<long>(
                name: "PaymentActiveId",
                table: "RentalRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentRentalRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalRequestId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DeleteStatus = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRentalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRentalRequests_RentalRequests_RentalRequestId",
                        column: x => x.RentalRequestId,
                        principalTable: "RentalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 3, 29, 11, 13, 5, 996, DateTimeKind.Utc).AddTicks(2924));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRentalRequests_RentalRequestId",
                table: "PaymentRentalRequests",
                column: "RentalRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentRentalRequests");

            migrationBuilder.DropColumn(
                name: "PaymentActiveId",
                table: "RentalRequests");

            migrationBuilder.AddColumn<bool>(
                name: "AuthorsViewingStatus",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecipientsViewingStatus",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 3, 23, 14, 38, 19, 945, DateTimeKind.Utc).AddTicks(6244));
        }
    }
}
