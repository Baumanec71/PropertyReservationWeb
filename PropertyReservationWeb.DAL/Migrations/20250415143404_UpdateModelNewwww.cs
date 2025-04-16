using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelNewwww : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckInTime",
                table: "RentalRequests",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckOutTime",
                table: "RentalRequests",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<decimal>(
                name: "FixedDepositAmount",
                table: "RentalRequests",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsAfterPhotosUploaded",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBeforePhotosUploaded",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhotoSkippedByLandlord",
                table: "RentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentActiveDepositId",
                table: "RentalRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReservationChangeRequestId",
                table: "RentalRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreate",
                table: "Photos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPayment",
                table: "PaymentRentalRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ReservationChangeRequestId",
                table: "PaymentRentalRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReservationChangeRequestId1",
                table: "PaymentRentalRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingPhotos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdRentalRequest = table.Column<long>(type: "bigint", nullable: false),
                    ValuePhoto = table.Column<byte[]>(type: "bytea", nullable: false),
                    Before = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteStatus = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPhotos_RentalRequests_IdRentalRequest",
                        column: x => x.IdRentalRequest,
                        principalTable: "RentalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conflicts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalRequestId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ResolvedByAdminId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateResolved = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conflicts_RentalRequests_RentalRequestId",
                        column: x => x.RentalRequestId,
                        principalTable: "RentalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conflicts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conflicts_Users_ResolvedByAdminId",
                        column: x => x.ResolvedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReservationChangeRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalRequestId = table.Column<long>(type: "bigint", nullable: false),
                    NewStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewFinishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewFixedPrepaymentAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NewFixedDepositAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationChangeRequests_RentalRequests_RentalRequestId",
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
                value: new DateTime(2025, 4, 15, 14, 34, 4, 136, DateTimeKind.Utc).AddTicks(5650));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRentalRequests_ReservationChangeRequestId",
                table: "PaymentRentalRequests",
                column: "ReservationChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRentalRequests_ReservationChangeRequestId1",
                table: "PaymentRentalRequests",
                column: "ReservationChangeRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPhotos_IdRentalRequest",
                table: "BookingPhotos",
                column: "IdRentalRequest");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_CreatedByUserId",
                table: "Conflicts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_RentalRequestId",
                table: "Conflicts",
                column: "RentalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_ResolvedByAdminId",
                table: "Conflicts",
                column: "ResolvedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationChangeRequests_RentalRequestId",
                table: "ReservationChangeRequests",
                column: "RentalRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRentalRequests_ReservationChangeRequests_Reservation~",
                table: "PaymentRentalRequests",
                column: "ReservationChangeRequestId",
                principalTable: "ReservationChangeRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRentalRequests_ReservationChangeRequests_Reservatio~1",
                table: "PaymentRentalRequests",
                column: "ReservationChangeRequestId1",
                principalTable: "ReservationChangeRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRentalRequests_ReservationChangeRequests_Reservation~",
                table: "PaymentRentalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRentalRequests_ReservationChangeRequests_Reservatio~1",
                table: "PaymentRentalRequests");

            migrationBuilder.DropTable(
                name: "BookingPhotos");

            migrationBuilder.DropTable(
                name: "Conflicts");

            migrationBuilder.DropTable(
                name: "ReservationChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRentalRequests_ReservationChangeRequestId",
                table: "PaymentRentalRequests");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRentalRequests_ReservationChangeRequestId1",
                table: "PaymentRentalRequests");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "FixedDepositAmount",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "IsAfterPhotosUploaded",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "IsBeforePhotosUploaded",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "IsPhotoSkippedByLandlord",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "PaymentActiveDepositId",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "ReservationChangeRequestId",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "DateCreate",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "IsPayment",
                table: "PaymentRentalRequests");

            migrationBuilder.DropColumn(
                name: "ReservationChangeRequestId",
                table: "PaymentRentalRequests");

            migrationBuilder.DropColumn(
                name: "ReservationChangeRequestId1",
                table: "PaymentRentalRequests");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 4, 10, 17, 15, 27, 950, DateTimeKind.Utc).AddTicks(5764));
        }
    }
}
