using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarPark.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleReg = table.Column<string>(type: "text", nullable: false),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    SpaceNumber = table.Column<int>(type: "integer", nullable: false),
                    TimeIn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VehicleCharge = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpaceNumber = table.Column<int>(type: "integer", nullable: false),
                    IsOccupied = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSpaces", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ParkingSpaces",
                columns: new[] { "Id", "IsOccupied", "SpaceNumber" },
                values: new object[,]
                {
                    { 1, false, 1 },
                    { 2, false, 2 },
                    { 3, false, 3 },
                    { 4, false, 4 },
                    { 5, false, 5 },
                    { 6, false, 6 },
                    { 7, false, 7 },
                    { 8, false, 8 },
                    { 9, false, 9 },
                    { 10, false, 10 },
                    { 11, false, 11 },
                    { 12, false, 12 },
                    { 13, false, 13 },
                    { 14, false, 14 },
                    { 15, false, 15 },
                    { 16, false, 16 },
                    { 17, false, 17 },
                    { 18, false, 18 },
                    { 19, false, 19 },
                    { 20, false, 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingRecords");

            migrationBuilder.DropTable(
                name: "ParkingSpaces");
        }
    }
}
