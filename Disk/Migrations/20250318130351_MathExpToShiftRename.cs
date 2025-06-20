﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations;

/// <inheritdoc />
public partial class MathExpToShiftRename : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.RenameColumn(
            name: "ares_math_exp_y",
            table: "attempt_result",
            newName: "ares_shift_y");

        _ = migrationBuilder.RenameColumn(
            name: "ares_math_exp_x",
            table: "attempt_result",
            newName: "ares_shift_x");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.RenameColumn(
            name: "ares_shift_y",
            table: "attempt_result",
            newName: "ares_math_exp_y");

        _ = migrationBuilder.RenameColumn(
            name: "ares_shift_x",
            table: "attempt_result",
            newName: "ares_math_exp_x");
    }
}
