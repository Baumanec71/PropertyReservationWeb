using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsRentals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedPrepaymentAmount",
                table: "Advertisements");

            migrationBuilder.AddColumn<decimal>(
                name: "FixedPrepaymentAmount",
                table: "RentalRequests",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsCalculated",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 4, 10, 17, 15, 27, 950, DateTimeKind.Utc).AddTicks(5764));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedPrepaymentAmount",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "IsCalculated",
                table: "RentalRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "FixedPrepaymentAmount",
                table: "Advertisements",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 4, 8, 13, 26, 24, 769, DateTimeKind.Utc).AddTicks(3001));
        }
    }
}
