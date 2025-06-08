using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class RemoveUnqSessionPatMapDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropIndex(
            name: "IX_UNQ_session_date_map_id_patient_id",
            table: "session");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateIndex(
            name: "IX_UNQ_session_date_map_id_patient_id",
            table: "session",
            columns: new[] { "ses_date", "ses_map", "ses_patient" },
            unique: true);
    }
}
