using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DateOfRegistration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObjectType = table.Column<int>(type: "integer", nullable: false),
                    AdressCoordinates = table.Column<Point>(type: "geometry", nullable: false, defaultValueSql: "ST_GeomFromText('POINT(0 0)', 4326)"),
                    AdressName = table.Column<string>(type: "text", nullable: false),
                    PlacementStatus = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TypeOfRental = table.Column<int>(type: "integer", nullable: false),
                    TotalArea = table.Column<long>(type: "bigint", nullable: false),
                    RentalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FixedPrepaymentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberOfRooms = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfBeds = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfBathrooms = table.Column<long>(type: "bigint", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    NumberOfPromotionPoints = table.Column<long>(type: "bigint", nullable: false),
                    IdAuthor = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advertisements_Users_IdAuthor",
                        column: x => x.IdAuthor,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationRooms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    IdOneUser = table.Column<long>(type: "bigint", nullable: false),
                    IdTwoUser = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationRooms_Users_IdOneUser",
                        column: x => x.IdOneUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationRooms_Users_IdTwoUser",
                        column: x => x.IdTwoUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAdvertisement = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Advertisements_IdAdvertisement",
                        column: x => x.IdAdvertisement,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rental_requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAuthorRentalRequest = table.Column<long>(type: "bigint", nullable: false),
                    IdNeedAdvertisement = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rental_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rental_requests_Advertisements_IdNeedAdvertisement",
                        column: x => x.IdNeedAdvertisement,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rental_requests_Users_IdAuthorRentalRequest",
                        column: x => x.IdAuthorRentalRequest,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdConversationRoom = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_ConversationRooms_IdConversationRoom",
                        column: x => x.IdConversationRoom,
                        principalTable: "ConversationRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdNeedRentalRequest = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Rental_requests_IdNeedRentalRequest",
                        column: x => x.IdNeedRentalRequest,
                        principalTable: "Rental_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfRegistration", "Email", "Name", "Password", "PhoneNumber", "Rating", "Role", "Status" },
                values: new object[] { 1L, new DateTime(2024, 12, 8, 13, 33, 10, 742, DateTimeKind.Utc).AddTicks(5260), "admin@example.com", "Andrey", "123456", "89992341221", 0.0, 2, false });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementConveniences_IdAdvertisement",
                table: "AdvertisementConveniences",
                column: "IdAdvertisement");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementConveniences_IdConvenience",
                table: "AdvertisementConveniences",
                column: "IdConvenience");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_IdAuthor",
                table: "Advertisements",
                column: "IdAuthor");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationRooms_IdOneUser",
                table: "ConversationRooms",
                column: "IdOneUser");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationRooms_IdTwoUser",
                table: "ConversationRooms",
                column: "IdTwoUser");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IdConversationRoom",
                table: "Messages",
                column: "IdConversationRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_IdAdvertisement",
                table: "Photos",
                column: "IdAdvertisement");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_requests_IdAuthorRentalRequest",
                table: "Rental_requests",
                column: "IdAuthorRentalRequest");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_requests_IdNeedAdvertisement",
                table: "Rental_requests",
                column: "IdNeedAdvertisement");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IdNeedRentalRequest",
                table: "Reviews",
                column: "IdNeedRentalRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementConveniences");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Conveniences");

            migrationBuilder.DropTable(
                name: "ConversationRooms");

            migrationBuilder.DropTable(
                name: "Rental_requests");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
