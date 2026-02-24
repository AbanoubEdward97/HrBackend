using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrBackend.Migrations
{
    /// <inheritdoc />
    public partial class addinggeneralsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.CreateTable(
                name: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OvertimeCalculationMethod = table.Column<int>(type: "int", nullable: false),
                    OvertimeValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductionCalculationMethod = table.Column<int>(type: "int", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WeeklyOfDay1 = table.Column<int>(type: "int", nullable: false),
                    WeeklyOfDay2 = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GeneralSettings",
                columns: new[] { "Id", "DeductionCalculationMethod", "DeductionValue", "LastUpdated", "OvertimeCalculationMethod", "OvertimeValue", "WeeklyOfDay1", "WeeklyOfDay2" },
                values: new object[] { 1, 1, 30m, new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2m, 5, 6 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSettings");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "Key", "Screen" },
                values: new object[,]
                {
                    { 1, "View", "Employees.View", "Employees" },
                    { 2, "Add", "Employees.Add", "Employees" },
                    { 3, "Edit", "Employees.Edit", "Employees" },
                    { 4, "Delete", "Employees.Delete", "Employees" }
                });
        }
    }
}
