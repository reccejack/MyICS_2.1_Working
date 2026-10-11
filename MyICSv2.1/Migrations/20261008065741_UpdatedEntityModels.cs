using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyICSv2._1.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntityModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Devices",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeviceAverage",
                table: "Devices",
                type: "TEXT",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceSecurity",
                table: "Devices",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReadingType",
                table: "Devices",
                type: "TEXT",
                maxLength: 5,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceAverage",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceSecurity",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ReadingType",
                table: "Devices");
        }
    }
}
