using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rental_requests_Advertisements_IdNeedAdvertisement",
                table: "Rental_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Rental_requests_Users_IdAuthorRentalRequest",
                table: "Rental_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Rental_requests_IdNeedRentalRequest",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rental_requests",
                table: "Rental_requests");

            migrationBuilder.RenameTable(
                name: "Rental_requests",
                newName: "RentalRequests");

            migrationBuilder.RenameIndex(
                name: "IX_Rental_requests_IdNeedAdvertisement",
                table: "RentalRequests",
                newName: "IX_RentalRequests_IdNeedAdvertisement");

            migrationBuilder.RenameIndex(
                name: "IX_Rental_requests_IdAuthorRentalRequest",
                table: "RentalRequests",
                newName: "IX_RentalRequests_IdAuthorRentalRequest");

            migrationBuilder.AddColumn<bool>(
                name: "AuthorsViewingStatus",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfCreation",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsTheLandlord",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecipientsViewingStatus",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RentalRequestId",
                table: "Reviews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "StatusDel",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TheQualityOfTheTransaction",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "RentalRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AuthorsViewingStatus",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingFinishDate",
                table: "RentalRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingStartDate",
                table: "RentalRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataChangeStatus",
                table: "RentalRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "DeleteStatus",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentalRequests",
                table: "RentalRequests",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2024, 12, 9, 13, 59, 24, 751, DateTimeKind.Utc).AddTicks(5223));

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequests_Advertisements_IdNeedAdvertisement",
                table: "RentalRequests",
                column: "IdNeedAdvertisement",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequests_Users_IdAuthorRentalRequest",
                table: "RentalRequests",
                column: "IdAuthorRentalRequest",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RentalRequests_IdNeedRentalRequest",
                table: "Reviews",
                column: "IdNeedRentalRequest",
                principalTable: "RentalRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequests_Advertisements_IdNeedAdvertisement",
                table: "RentalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequests_Users_IdAuthorRentalRequest",
                table: "RentalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RentalRequests_IdNeedRentalRequest",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentalRequests",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "AuthorsViewingStatus",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsTheLandlord",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "RecipientsViewingStatus",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "RentalRequestId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "StatusDel",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TheQualityOfTheTransaction",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "AuthorsViewingStatus",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "BookingFinishDate",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "BookingStartDate",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "DataChangeStatus",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "DeleteStatus",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "RecipientsViewingStatus",
                table: "RentalRequests");

            migrationBuilder.RenameTable(
                name: "RentalRequests",
                newName: "Rental_requests");

            migrationBuilder.RenameIndex(
                name: "IX_RentalRequests_IdNeedAdvertisement",
                table: "Rental_requests",
                newName: "IX_Rental_requests_IdNeedAdvertisement");

            migrationBuilder.RenameIndex(
                name: "IX_RentalRequests_IdAuthorRentalRequest",
                table: "Rental_requests",
                newName: "IX_Rental_requests_IdAuthorRentalRequest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rental_requests",
                table: "Rental_requests",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2024, 12, 8, 13, 33, 10, 742, DateTimeKind.Utc).AddTicks(5260));

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_requests_Advertisements_IdNeedAdvertisement",
                table: "Rental_requests",
                column: "IdNeedAdvertisement",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_requests_Users_IdAuthorRentalRequest",
                table: "Rental_requests",
                column: "IdAuthorRentalRequest",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Rental_requests_IdNeedRentalRequest",
                table: "Reviews",
                column: "IdNeedRentalRequest",
                principalTable: "Rental_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
