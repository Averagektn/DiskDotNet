using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class PatientDeleteCascadeOperation_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_session_appointment_ses_appointment",
                table: "session");

            migrationBuilder.AddForeignKey(
                name: "FK_session_appointment_ses_appointment",
                table: "session",
                column: "ses_appointment",
                principalTable: "appointment",
                principalColumn: "app_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_session_appointment_ses_appointment",
                table: "session");

            migrationBuilder.AddForeignKey(
                name: "FK_session_appointment_ses_appointment",
                table: "session",
                column: "ses_appointment",
                principalTable: "appointment",
                principalColumn: "app_id");
        }
    }
}
