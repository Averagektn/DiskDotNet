using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class RemovedDoctor : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_appointment_doctor_app_doctor",
            table: "appointment");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_map_doctor_map_created_by",
            table: "map");

        _ = migrationBuilder.DropTable(
            name: "doctor");

        _ = migrationBuilder.DropIndex(
            name: "IX_map_map_created_by",
            table: "map");

        _ = migrationBuilder.DropIndex(
            name: "IX_appointment_app_doctor",
            table: "appointment");

        _ = migrationBuilder.DropColumn(
            name: "map_created_by",
            table: "map");

        _ = migrationBuilder.DropColumn(
            name: "app_doctor",
            table: "appointment");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<long>(
            name: "map_created_by",
            table: "map",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);

        _ = migrationBuilder.AddColumn<long>(
            name: "app_doctor",
            table: "appointment",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);

        _ = migrationBuilder.CreateTable(
            name: "doctor",
            columns: table => new
            {
                doc_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                doc_name = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                doc_password = table.Column<string>(type: "TEXT", nullable: false),
                doc_patronymic = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                doc_surname = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_doctor", x => x.doc_id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_map_map_created_by",
            table: "map",
            column: "map_created_by");

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_doctor",
            table: "appointment",
            column: "app_doctor");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_appointment_doctor_app_doctor",
            table: "appointment",
            column: "app_doctor",
            principalTable: "doctor",
            principalColumn: "doc_id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_map_doctor_map_created_by",
            table: "map",
            column: "map_created_by",
            principalTable: "doctor",
            principalColumn: "doc_id",
            onDelete: ReferentialAction.Cascade);
    }
}
