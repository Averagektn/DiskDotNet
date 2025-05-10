using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class EllipseConvexHullAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "pit_convex_hull_area",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_ellipse_area",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_full_path_convex_hull_area",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            _ = migrationBuilder.AddColumn<double>(
                name: "pit_full_path_ellipse_area",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "pit_convex_hull_area",
                table: "path_to_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_ellipse_area",
                table: "path_to_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_convex_hull_area",
                table: "path_in_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_ellipse_area",
                table: "path_in_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_full_path_convex_hull_area",
                table: "path_in_target");

            _ = migrationBuilder.DropColumn(
                name: "pit_full_path_ellipse_area",
                table: "path_in_target");
        }
    }
}
