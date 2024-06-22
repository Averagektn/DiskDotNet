using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_doctor_app_doctor",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_map_doctor_map_created_by",
                table: "map");

            migrationBuilder.DropTable(
                name: "doctor");

            migrationBuilder.DropIndex(
                name: "IX_map_map_created_by",
                table: "map");

            migrationBuilder.DropIndex(
                name: "IX_appointment_app_doctor",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "map_created_by",
                table: "map");

            migrationBuilder.DropColumn(
                name: "app_doctor",
                table: "appointment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "map_created_by",
                table: "map",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "app_doctor",
                table: "appointment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
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
                    table.PrimaryKey("PK_doctor", x => x.doc_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_map_created_by",
                table: "map",
                column: "map_created_by");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_app_doctor",
                table: "appointment",
                column: "app_doctor");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_doctor_app_doctor",
                table: "appointment",
                column: "app_doctor",
                principalTable: "doctor",
                principalColumn: "doc_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_map_doctor_map_created_by",
                table: "map",
                column: "map_created_by",
                principalTable: "doctor",
                principalColumn: "doc_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
