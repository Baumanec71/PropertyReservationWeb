using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PropertyReservationWeb.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Убираем автоинкремент с столбца "Id" в таблице "PaymentRentalRequests"
            migrationBuilder.Sql("ALTER TABLE \"PaymentRentalRequests\" ALTER COLUMN \"Id\" DROP IDENTITY;");

            // Изменяем тип столбца "PaymentActiveId" на string в таблице "RentalRequests"
            migrationBuilder.AlterColumn<string>(
                name: "PaymentActiveId",
                table: "RentalRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // Изменяем тип столбца "Id" на string в таблице "PaymentRentalRequests"
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PaymentRentalRequests",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            // Добавляем новый столбец "Url" в таблицу "PaymentRentalRequests"
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "PaymentRentalRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Обновляем данные в таблице "Users"
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 3, 31, 15, 35, 36, 253, DateTimeKind.Utc).AddTicks(9196));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем столбец "Url" из таблицы "PaymentRentalRequests"
            migrationBuilder.DropColumn(
                name: "Url",
                table: "PaymentRentalRequests");

            // Восстанавливаем старый тип для "PaymentActiveId"
            migrationBuilder.AlterColumn<long>(
                name: "PaymentActiveId",
                table: "RentalRequests",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Восстанавливаем тип столбца "Id" обратно на bigint и восстанавливаем автоинкремент
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PaymentRentalRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // Обновляем данные в таблице "Users" на старое значение
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateOfRegistration",
                value: new DateTime(2025, 3, 30, 12, 37, 44, 241, DateTimeKind.Utc).AddTicks(8055));
        }
    }
}
