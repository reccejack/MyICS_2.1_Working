using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyICSv2._1.Migrations
{
    /// <inheritdoc />
    public partial class AddSubProcessProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubProcess",
                table: "Devices",
                type: "TEXT",
                maxLength: 12,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubProcess",
                table: "Devices");
        }
    }
}
