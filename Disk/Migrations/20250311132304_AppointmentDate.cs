using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class AppointmentDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "app_date",
            table: "appointment",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_date_app_map",
            table: "appointment",
            columns: new[] { "app_date", "app_map" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropIndex(
            name: "IX_appointment_app_date_app_map",
            table: "appointment");

        _ = migrationBuilder.DropColumn(
            name: "app_date",
            table: "appointment");
    }
}
