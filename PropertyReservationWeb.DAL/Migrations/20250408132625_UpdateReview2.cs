using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReview2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentalRequestId",
                table: "Reviews");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 4, 8, 13, 26, 24, 769, DateTimeKind.Utc).AddTicks(3001));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RentalRequestId",
                table: "Reviews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 4, 7, 17, 5, 27, 500, DateTimeKind.Utc).AddTicks(9193));
        }
    }
}
