using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class SessionAndAppoinmentRename : Migration
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
            name: "FK_session_appointment_ses_appointment",
            table: "session");

        _ = migrationBuilder.DropTable(
            name: "appointment");

        _ = migrationBuilder.DropTable(
            name: "session_result");

        _ = migrationBuilder.DropIndex(
            name: "IX_session_ses_appointment",
            table: "session");

        _ = migrationBuilder.DropIndex(
            name: "IX_UNQ_session_ses_log_file_path",
            table: "session");

        _ = migrationBuilder.DropColumn(
            name: "ses_appointment",
            table: "session");

        _ = migrationBuilder.DropColumn(
            name: "ses_date_time",
            table: "session");

        _ = migrationBuilder.DropColumn(
            name: "ses_max_x_angle",
            table: "session");

        _ = migrationBuilder.DropColumn(
            name: "ses_max_y_angle",
            table: "session");

        _ = migrationBuilder.RenameColumn(
            name: "ses_target_radius",
            table: "session",
            newName: "ses_patient");

        _ = migrationBuilder.RenameColumn(
            name: "ses_log_file_path",
            table: "session",
            newName: "ses_date");

        _ = migrationBuilder.RenameColumn(
            name: "ses_cursor_radius",
            table: "session",
            newName: "ses_map");

        _ = migrationBuilder.CreateTable(
            name: "attempt",
            columns: table => new
            {
                att_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                att_max_x_angle = table.Column<float>(type: "REAL", nullable: false),
                att_max_y_angle = table.Column<float>(type: "REAL", nullable: false),
                att_cursor_radius = table.Column<int>(type: "INTEGER", nullable: false),
                att_target_radius = table.Column<int>(type: "INTEGER", nullable: false),
                att_log_file_path = table.Column<string>(type: "TEXT", nullable: false),
                att_date_time = table.Column<string>(type: "TEXT", nullable: false),
                att_session = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_attempt", x => x.att_id);
                _ = table.ForeignKey(
                    name: "FK_attempt_session_att_session",
                    column: x => x.att_session,
                    principalTable: "session",
                    principalColumn: "ses_id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "attempt_result",
            columns: table => new
            {
                ares_id = table.Column<long>(type: "INTEGER", nullable: false),
                ares_math_exp_y = table.Column<double>(type: "REAL", nullable: false),
                ares_math_exp_x = table.Column<double>(type: "REAL", nullable: false),
                ares_deviation_x = table.Column<double>(type: "REAL", nullable: false),
                ares_deviation_y = table.Column<double>(type: "REAL", nullable: false),
                ares_score = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_attempt_result", x => x.ares_id);
                _ = table.ForeignKey(
                    name: "FK_attempt_result_attempt_ares_id",
                    column: x => x.ares_id,
                    principalTable: "attempt",
                    principalColumn: "att_id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_map",
            table: "session",
            column: "ses_map");

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_patient",
            table: "session",
            column: "ses_patient");

        _ = migrationBuilder.CreateIndex(
            name: "IX_UNQ_session_date_map_id_patient_id",
            table: "session",
            columns: new[] { "ses_date", "ses_map", "ses_patient" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_attempt_att_session",
            table: "attempt",
            column: "att_session");

        _ = migrationBuilder.CreateIndex(
            name: "IX_UNQ_attempt_att_log_file_path",
            table: "attempt",
            column: "att_log_file_path",
            unique: true);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_path_in_target_attempt_pit_session",
            table: "path_in_target",
            column: "pit_session",
            principalTable: "attempt",
            principalColumn: "att_id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_path_to_target_attempt_ptt_session",
            table: "path_to_target",
            column: "ptt_session",
            principalTable: "attempt",
            principalColumn: "att_id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_session_map_ses_map",
            table: "session",
            column: "ses_map",
            principalTable: "map",
            principalColumn: "map_id",
            onDelete: ReferentialAction.Restrict);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_session_patient_ses_patient",
            table: "session",
            column: "ses_patient",
            principalTable: "patient",
            principalColumn: "pat_id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_path_in_target_attempt_pit_session",
            table: "path_in_target");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_path_to_target_attempt_ptt_session",
            table: "path_to_target");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_session_map_ses_map",
            table: "session");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_session_patient_ses_patient",
            table: "session");

        _ = migrationBuilder.DropTable(
            name: "attempt_result");

        _ = migrationBuilder.DropTable(
            name: "attempt");

        _ = migrationBuilder.DropIndex(
            name: "IX_session_ses_map",
            table: "session");

        _ = migrationBuilder.DropIndex(
            name: "IX_session_ses_patient",
            table: "session");

        _ = migrationBuilder.DropIndex(
            name: "IX_UNQ_session_date_map_id_patient_id",
            table: "session");

        _ = migrationBuilder.RenameColumn(
            name: "ses_patient",
            table: "session",
            newName: "ses_target_radius");

        _ = migrationBuilder.RenameColumn(
            name: "ses_map",
            table: "session",
            newName: "ses_cursor_radius");

        _ = migrationBuilder.RenameColumn(
            name: "ses_date",
            table: "session",
            newName: "ses_log_file_path");

        _ = migrationBuilder.AddColumn<long>(
            name: "ses_appointment",
            table: "session",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);

        _ = migrationBuilder.AddColumn<string>(
            name: "ses_date_time",
            table: "session",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        _ = migrationBuilder.AddColumn<float>(
            name: "ses_max_x_angle",
            table: "session",
            type: "REAL",
            nullable: false,
            defaultValue: 0f);

        _ = migrationBuilder.AddColumn<float>(
            name: "ses_max_y_angle",
            table: "session",
            type: "REAL",
            nullable: false,
            defaultValue: 0f);

        _ = migrationBuilder.CreateTable(
            name: "appointment",
            columns: table => new
            {
                app_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                app_map = table.Column<long>(type: "INTEGER", nullable: false),
                app_patient = table.Column<long>(type: "INTEGER", nullable: false),
                app_date = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_appointment", x => x.app_id);
                _ = table.ForeignKey(
                    name: "FK_appointment_map_app_map",
                    column: x => x.app_map,
                    principalTable: "map",
                    principalColumn: "map_id",
                    onDelete: ReferentialAction.Restrict);
                _ = table.ForeignKey(
                    name: "FK_appointment_patient_app_patient",
                    column: x => x.app_patient,
                    principalTable: "patient",
                    principalColumn: "pat_id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "session_result",
            columns: table => new
            {
                sres_id = table.Column<long>(type: "INTEGER", nullable: false),
                sres_deviation_x = table.Column<double>(type: "REAL", nullable: false),
                sres_deviation_y = table.Column<double>(type: "REAL", nullable: false),
                sres_math_exp_x = table.Column<double>(type: "REAL", nullable: false),
                sres_math_exp_y = table.Column<double>(type: "REAL", nullable: false),
                sres_score = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_session_result", x => x.sres_id);
                _ = table.ForeignKey(
                    name: "FK_session_result_session_sres_id",
                    column: x => x.sres_id,
                    principalTable: "session",
                    principalColumn: "ses_id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_appointment",
            table: "session",
            column: "ses_appointment");

        _ = migrationBuilder.CreateIndex(
            name: "IX_UNQ_session_ses_log_file_path",
            table: "session",
            column: "ses_log_file_path",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_map",
            table: "appointment",
            column: "app_map");

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_patient",
            table: "appointment",
            column: "app_patient");

        _ = migrationBuilder.CreateIndex(
            name: "IX_UNQ_appointment_date_map_id_patient_id",
            table: "appointment",
            columns: new[] { "app_date", "app_map", "app_patient" },
            unique: true);

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
            name: "FK_session_appointment_ses_appointment",
            table: "session",
            column: "ses_appointment",
            principalTable: "appointment",
            principalColumn: "app_id",
            onDelete: ReferentialAction.Cascade);
    }
}
