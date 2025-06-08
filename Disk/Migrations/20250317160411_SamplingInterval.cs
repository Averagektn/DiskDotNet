using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class SamplingInterval : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_path_in_target_attempt_pit_session",
            table: "path_in_target");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_path_to_target_attempt_ptt_session",
            table: "path_to_target");

        _ = migrationBuilder.RenameColumn(
            name: "ptt_session",
            table: "path_to_target",
            newName: "ptt_attempt");

        _ = migrationBuilder.RenameColumn(
            name: "pit_session",
            table: "path_in_target",
            newName: "pit_attempt");

        _ = migrationBuilder.AddColumn<int>(
            name: "att_sampling_interval",
            table: "attempt",
            type: "INTEGER",
            nullable: false,
            defaultValue: 50);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_path_in_target_attempt_pit_attempt",
            table: "path_in_target",
            column: "pit_attempt",
            principalTable: "attempt",
            principalColumn: "att_id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_path_to_target_attempt_ptt_attempt",
            table: "path_to_target",
            column: "ptt_attempt",
            principalTable: "attempt",
            principalColumn: "att_id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_path_in_target_attempt_pit_attempt",
            table: "path_in_target");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_path_to_target_attempt_ptt_attempt",
            table: "path_to_target");

        _ = migrationBuilder.DropColumn(
            name: "att_sampling_interval",
            table: "attempt");

        _ = migrationBuilder.RenameColumn(
            name: "ptt_attempt",
            table: "path_to_target",
            newName: "ptt_session");

        _ = migrationBuilder.RenameColumn(
            name: "pit_attempt",
            table: "path_in_target",
            newName: "pit_session");

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
    }
}
