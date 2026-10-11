using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyICSv2._1.Migrations
{
    /// <inheritdoc />
    public partial class ProcessesTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    ProcessId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessStatus = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Connection = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    IsAlarm = table.Column<bool>(type: "INTEGER", nullable: true),
                    ProcessReport = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.ProcessId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Processes");
        }
    }
}
