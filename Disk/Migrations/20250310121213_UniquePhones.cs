using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class UniquePhones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_session_map_ses_map",
                table: "session");

            migrationBuilder.DropIndex(
                name: "IX_session_ses_map",
                table: "session");

            migrationBuilder.DropColumn(
                name: "ses_map",
                table: "session");

            migrationBuilder.DropColumn(
                name: "app_date_time",
                table: "appointment");

            migrationBuilder.AddColumn<long>(
                name: "app_map",
                table: "appointment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_patient_pat_phone_home",
                table: "patient",
                column: "pat_phone_home",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_pat_phone_mobile",
                table: "patient",
                column: "pat_phone_mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appointment_app_map",
                table: "appointment",
                column: "app_map");

            migrationBuilder.AddForeignKey(
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
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_map_app_map",
                table: "appointment");

            migrationBuilder.DropIndex(
                name: "IX_patient_pat_phone_home",
                table: "patient");

            migrationBuilder.DropIndex(
                name: "IX_patient_pat_phone_mobile",
                table: "patient");

            migrationBuilder.DropIndex(
                name: "IX_appointment_app_map",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "app_map",
                table: "appointment");

            migrationBuilder.AddColumn<long>(
                name: "ses_map",
                table: "session",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "app_date_time",
                table: "appointment",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_session_ses_map",
                table: "session",
                column: "ses_map");

            migrationBuilder.AddForeignKey(
                name: "FK_session_map_ses_map",
                table: "session",
                column: "ses_map",
                principalTable: "map",
                principalColumn: "map_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
