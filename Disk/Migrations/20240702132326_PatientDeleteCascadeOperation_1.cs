using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class PatientDeleteCascadeOperation_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_path_in_target_session_pit_session",
                table: "path_in_target");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_path_to_target_session_ptt_session",
                table: "path_to_target");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_session_result_session_sres_id",
                table: "session_result");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_path_in_target_session_pit_session",
                table: "path_in_target",
                column: "pit_session",
                principalTable: "session",
                principalColumn: "ses_id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_path_to_target_session_ptt_session",
                table: "path_to_target",
                column: "ptt_session",
                principalTable: "session",
                principalColumn: "ses_id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_session_result_session_sres_id",
                table: "session_result",
                column: "sres_id",
                principalTable: "session",
                principalColumn: "ses_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_path_in_target_session_pit_session",
                table: "path_in_target");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_path_to_target_session_ptt_session",
                table: "path_to_target");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_session_result_session_sres_id",
                table: "session_result");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_path_in_target_session_pit_session",
                table: "path_in_target",
                column: "pit_session",
                principalTable: "session",
                principalColumn: "ses_id",
                onDelete: ReferentialAction.Restrict);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_path_to_target_session_ptt_session",
                table: "path_to_target",
                column: "ptt_session",
                principalTable: "session",
                principalColumn: "ses_id",
                onDelete: ReferentialAction.Restrict);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_session_result_session_sres_id",
                table: "session_result",
                column: "sres_id",
                principalTable: "session",
                principalColumn: "ses_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
