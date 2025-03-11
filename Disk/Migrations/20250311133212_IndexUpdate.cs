using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class IndexUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_appointment_app_date_app_map",
                table: "appointment");

            migrationBuilder.RenameIndex(
                name: "IX_session_ses_log_file_path",
                table: "session",
                newName: "IX_UNQ_session_ses_log_file_path");

            migrationBuilder.RenameIndex(
                name: "IX_patient_pat_phone_mobile",
                table: "patient",
                newName: "IX_UNQ_phone_mobile");

            migrationBuilder.RenameIndex(
                name: "IX_patient_pat_phone_home",
                table: "patient",
                newName: "IX_UNQ_phone_home");

            migrationBuilder.RenameIndex(
                name: "IX_map_map_name",
                table: "map",
                newName: "IX_UNQ_map_map_name");

            migrationBuilder.AddColumn<string>(
                name: "map_description",
                table: "map",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UNQ_appointment_date_map_id_patient_id",
                table: "appointment",
                columns: new[] { "app_date", "app_map", "app_patient" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UNQ_appointment_date_map_id_patient_id",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "map_description",
                table: "map");

            migrationBuilder.RenameIndex(
                name: "IX_UNQ_session_ses_log_file_path",
                table: "session",
                newName: "IX_session_ses_log_file_path");

            migrationBuilder.RenameIndex(
                name: "IX_UNQ_phone_mobile",
                table: "patient",
                newName: "IX_patient_pat_phone_mobile");

            migrationBuilder.RenameIndex(
                name: "IX_UNQ_phone_home",
                table: "patient",
                newName: "IX_patient_pat_phone_home");

            migrationBuilder.RenameIndex(
                name: "IX_UNQ_map_map_name",
                table: "map",
                newName: "IX_map_map_name");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_app_date_app_map",
                table: "appointment",
                columns: new[] { "app_date", "app_map" },
                unique: true);
        }
    }
}
