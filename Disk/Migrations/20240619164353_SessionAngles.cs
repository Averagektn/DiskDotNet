using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class SessionAngles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "doctor",
            columns: table => new
            {
                doc_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                doc_name = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                doc_surname = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                doc_patronymic = table.Column<string>(type: "TEXT", nullable: true, collation: "NOCASE"),
                doc_password = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_doctor", x => x.doc_id);
            });

        _ = migrationBuilder.CreateTable(
            name: "patient",
            columns: table => new
            {
                pat_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                pat_name = table.Column<string>(type: "TEXT (20)", nullable: false, collation: "NOCASE"),
                pat_surname = table.Column<string>(type: "TEXT (30)", nullable: false, collation: "NOCASE"),
                pat_patronymic = table.Column<string>(type: "TEXT (30)", nullable: true, collation: "NOCASE"),
                pat_date_of_birth = table.Column<string>(type: "TEXT", nullable: false),
                pat_phone_mobile = table.Column<string>(type: "TEXT", nullable: false),
                pat_phone_home = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_patient", x => x.pat_id);
            });

        _ = migrationBuilder.CreateTable(
            name: "map",
            columns: table => new
            {
                map_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                map_coordinates_json = table.Column<string>(type: "TEXT", nullable: false),
                map_created_at_date_time = table.Column<string>(type: "TEXT", nullable: false),
                map_created_by = table.Column<long>(type: "INTEGER", nullable: false),
                map_name = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_map", x => x.map_id);
                _ = table.ForeignKey(
                    name: "FK_map_doctor_map_created_by",
                    column: x => x.map_created_by,
                    principalTable: "doctor",
                    principalColumn: "doc_id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "appointment",
            columns: table => new
            {
                app_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                app_date_time = table.Column<string>(type: "TEXT", nullable: false),
                app_doctor = table.Column<long>(type: "INTEGER", nullable: false),
                app_patient = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_appointment", x => x.app_id);
                _ = table.ForeignKey(
                    name: "FK_appointment_doctor_app_doctor",
                    column: x => x.app_doctor,
                    principalTable: "doctor",
                    principalColumn: "doc_id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_appointment_patient_app_patient",
                    column: x => x.app_patient,
                    principalTable: "patient",
                    principalColumn: "pat_id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "session",
            columns: table => new
            {
                ses_id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ses_map = table.Column<long>(type: "INTEGER", nullable: false),
                ses_max_x_angle = table.Column<float>(type: "REAL", nullable: false),
                ses_max_y_angle = table.Column<float>(type: "REAL", nullable: false),
                ses_log_file_path = table.Column<string>(type: "TEXT", nullable: false),
                ses_date_time = table.Column<string>(type: "TEXT", nullable: false),
                ses_appointment = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_session", x => x.ses_id);
                _ = table.ForeignKey(
                    name: "FK_session_appointment_ses_appointment",
                    column: x => x.ses_appointment,
                    principalTable: "appointment",
                    principalColumn: "app_id");
                _ = table.ForeignKey(
                    name: "FK_session_map_ses_map",
                    column: x => x.ses_map,
                    principalTable: "map",
                    principalColumn: "map_id",
                    onDelete: ReferentialAction.Restrict);
            });

        _ = migrationBuilder.CreateTable(
            name: "path_in_target",
            columns: table => new
            {
                pit_session = table.Column<long>(type: "INTEGER", nullable: false),
                pit_target_id = table.Column<long>(type: "INTEGER", nullable: false),
                pit_coordinates_json = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_path_in_target", x => new { x.pit_session, x.pit_target_id });
                _ = table.ForeignKey(
                    name: "FK_path_in_target_session_pit_session",
                    column: x => x.pit_session,
                    principalTable: "session",
                    principalColumn: "ses_id",
                    onDelete: ReferentialAction.Restrict);
            });

        _ = migrationBuilder.CreateTable(
            name: "path_to_target",
            columns: table => new
            {
                ptt_session = table.Column<long>(type: "INTEGER", nullable: false),
                ptt_target_num = table.Column<long>(type: "INTEGER", nullable: false),
                ptt_coordinates_json = table.Column<string>(type: "TEXT", nullable: false),
                ptt_ange_distance = table.Column<double>(type: "REAL", nullable: false),
                ptt_angle_speed = table.Column<double>(type: "REAL", nullable: false),
                ptt_approach_speed = table.Column<double>(type: "REAL", nullable: false),
                ptt_time = table.Column<double>(type: "REAL", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_path_to_target", x => new { x.ptt_session, x.ptt_target_num });
                _ = table.ForeignKey(
                    name: "FK_path_to_target_session_ptt_session",
                    column: x => x.ptt_session,
                    principalTable: "session",
                    principalColumn: "ses_id",
                    onDelete: ReferentialAction.Restrict);
            });

        _ = migrationBuilder.CreateTable(
            name: "session_result",
            columns: table => new
            {
                sres_id = table.Column<long>(type: "INTEGER", nullable: false),
                sres_math_exp = table.Column<double>(type: "REAL", nullable: false),
                sres_deviation = table.Column<double>(type: "REAL", nullable: false),
                sres_dispersion = table.Column<double>(type: "REAL", nullable: false),
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
                    onDelete: ReferentialAction.Restrict);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_doctor",
            table: "appointment",
            column: "app_doctor");

        _ = migrationBuilder.CreateIndex(
            name: "IX_appointment_app_patient",
            table: "appointment",
            column: "app_patient");

        _ = migrationBuilder.CreateIndex(
            name: "IX_map_map_created_by",
            table: "map",
            column: "map_created_by");

        _ = migrationBuilder.CreateIndex(
            name: "IX_map_map_name",
            table: "map",
            column: "map_name",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_appointment",
            table: "session",
            column: "ses_appointment");

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_log_file_path",
            table: "session",
            column: "ses_log_file_path",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_session_ses_map",
            table: "session",
            column: "ses_map");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "path_in_target");

        _ = migrationBuilder.DropTable(
            name: "path_to_target");

        _ = migrationBuilder.DropTable(
            name: "session_result");

        _ = migrationBuilder.DropTable(
            name: "session");

        _ = migrationBuilder.DropTable(
            name: "appointment");

        _ = migrationBuilder.DropTable(
            name: "map");

        _ = migrationBuilder.DropTable(
            name: "patient");

        _ = migrationBuilder.DropTable(
            name: "doctor");
    }
}
