using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class MathexpDeviationRebase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "pit_convex_hull_area",
                table: "path_to_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_ellipse_area",
                table: "path_to_target");

            _ = migrationBuilder.DropColumn(
                name: "ares_deviation_x",
                table: "attempt_result");

            _ = migrationBuilder.DropColumn(
                name: "ares_deviation_y",
                table: "attempt_result");

            _ = migrationBuilder.DropColumn(
                name: "ares_shift_x",
                table: "attempt_result");

            _ = migrationBuilder.DropColumn(
                name: "ares_shift_y",
                table: "attempt_result");

            _ = migrationBuilder.RenameColumn(
                name: "pit_full_path_ellipse_area",
                table: "path_in_target",
                newName: "pit_math_exp_y");

            _ = migrationBuilder.RenameColumn(
                name: "pit_full_path_convex_hull_area",
                table: "path_in_target",
                newName: "pit_math_exp_x");

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_deviation_x",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_deviation_y",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "pit_deviation_x",
                table: "path_in_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_deviation_y",
                table: "path_in_target");

            _ = migrationBuilder.RenameColumn(
                name: "pit_math_exp_y",
                table: "path_in_target",
                newName: "pit_full_path_ellipse_area");

            _ = migrationBuilder.RenameColumn(
                name: "pit_math_exp_x",
                table: "path_in_target",
                newName: "pit_full_path_convex_hull_area");

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_convex_hull_area",
                table: "path_to_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_ellipse_area",
                table: "path_to_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "ares_deviation_x",
                table: "attempt_result",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "ares_deviation_y",
                table: "attempt_result",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "ares_shift_x",
                table: "attempt_result",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "ares_shift_y",
                table: "attempt_result",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
