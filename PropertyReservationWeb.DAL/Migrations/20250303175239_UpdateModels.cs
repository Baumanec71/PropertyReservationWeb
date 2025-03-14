using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementAmenityes_Advertisements_IdAdvertisement",
                table: "AdvertisementAmenityes");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementAmenityes_Amenityes_IdAmenity",
                table: "AdvertisementAmenityes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Amenityes",
                table: "Amenityes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdvertisementAmenityes",
                table: "AdvertisementAmenityes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "TypeOfRental",
                table: "Advertisements");

            migrationBuilder.RenameTable(
                name: "Amenityes",
                newName: "Amenities");

            migrationBuilder.RenameTable(
                name: "AdvertisementAmenityes",
                newName: "AdvertisementAmenities");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertisementAmenityes_IdAmenity",
                table: "AdvertisementAmenities",
                newName: "IX_AdvertisementAmenities_IdAmenity");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertisementAmenityes_IdAdvertisement",
                table: "AdvertisementAmenities",
                newName: "IX_AdvertisementAmenities_IdAdvertisement");

            migrationBuilder.AddColumn<byte[]>(
                name: "ValuePhoto",
                table: "Photos",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<decimal>(
                name: "FixedPrepaymentAmount",
                table: "Advertisements",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "ApartmentNumber",
                table: "Advertisements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AmenityType",
                table: "Amenities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "AdvertisementAmenities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Amenities",
                table: "Amenities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdvertisementAmenities",
                table: "AdvertisementAmenities",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 3, 3, 17, 52, 38, 554, DateTimeKind.Utc).AddTicks(7244));

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementAmenities_Advertisements_IdAdvertisement",
                table: "AdvertisementAmenities",
                column: "IdAdvertisement",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementAmenities_Amenities_IdAmenity",
                table: "AdvertisementAmenities",
                column: "IdAmenity",
                principalTable: "Amenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementAmenities_Advertisements_IdAdvertisement",
                table: "AdvertisementAmenities");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementAmenities_Amenities_IdAmenity",
                table: "AdvertisementAmenities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Amenities",
                table: "Amenities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdvertisementAmenities",
                table: "AdvertisementAmenities");

            migrationBuilder.DropColumn(
                name: "ValuePhoto",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "AmenityType",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AdvertisementAmenities");

            migrationBuilder.RenameTable(
                name: "Amenities",
                newName: "Amenityes");

            migrationBuilder.RenameTable(
                name: "AdvertisementAmenities",
                newName: "AdvertisementAmenityes");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertisementAmenities_IdAmenity",
                table: "AdvertisementAmenityes",
                newName: "IX_AdvertisementAmenityes_IdAmenity");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertisementAmenities_IdAdvertisement",
                table: "AdvertisementAmenityes",
                newName: "IX_AdvertisementAmenityes_IdAdvertisement");

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

            migrationBuilder.AlterColumn<decimal>(
                name: "FixedPrepaymentAmount",
                table: "Advertisements",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "TypeOfRental",
                table: "Advertisements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Amenityes",
                table: "Amenityes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdvertisementAmenityes",
                table: "AdvertisementAmenityes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 2, 16, 14, 57, 28, 57, DateTimeKind.Utc).AddTicks(8037));

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementAmenityes_Advertisements_IdAdvertisement",
                table: "AdvertisementAmenityes",
                column: "IdAdvertisement",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementAmenityes_Amenityes_IdAmenity",
                table: "AdvertisementAmenityes",
                column: "IdAmenity",
                principalTable: "Amenityes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
