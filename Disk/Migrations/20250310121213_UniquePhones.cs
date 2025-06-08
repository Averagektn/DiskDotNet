using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class UniquePhones : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_session_map_ses_map",
            table: "session");

        _ = migrationBuilder.DropIndex(
            name: "IX_session_ses_map",
            table: "session");

        _ = migrationBuilder.DropColumn(
            name: "ses_map",
            table: "session");

        _ = migrationBuilder.DropColumn(
            name: "app_date_time",
            table: "appointment");

        _ = migrationBuilder.AddColumn<long>(
            name: "app_map",
            table: "appointment",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);

        _ = migrationBuilder.CreateIndex(
            name: "IX_patient_pat_phone_home",
            table: "patient",
            column: "pat_phone_home",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_patient_pat_phone_mobile",
            table: "patient",
            column: "pat_phone_mobile",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_map",
            table: "appointment",
            column: "app_map");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_appointment_map_app_map",
            table: "appointment",
            column: "app_map",
            principalTable: "map",
            principalColumn: "map_id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_appointment_map_app_map",
            table: "appointment");

        _ = migrationBuilder.DropIndex(
            name: "IX_patient_pat_phone_home",
            table: "patient");

        _ = migrationBuilder.DropIndex(
            name: "IX_patient_pat_phone_mobile",
            table: "patient");

        _ = migrationBuilder.DropIndex(
            name: "IX_appointment_app_map",
            table: "appointment");

        _ = migrationBuilder.DropColumn(
            name: "app_map",
            table: "appointment");

        _ = migrationBuilder.AddColumn<long>(
            name: "ses_map",
            table: "session",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);

        _ = migrationBuilder.AddColumn<string>(
            name: "app_date_time",
            table: "appointment",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_map",
            table: "session",
            column: "ses_map");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_session_map_ses_map",
            table: "session",
            column: "ses_map",
            principalTable: "map",
            principalColumn: "map_id",
            onDelete: ReferentialAction.Restrict);
    }
}
