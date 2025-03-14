using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class CheracteristicsAxesSplit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.RenameColumn(
                name: "sres_math_exp",
                table: "session_result",
                newName: "sres_math_exp_y");

            _ = migrationBuilder.RenameColumn(
                name: "sres_dispersion",
                table: "session_result",
                newName: "sres_math_exp_x");

            _ = migrationBuilder.RenameColumn(
                name: "sres_deviation",
                table: "session_result",
                newName: "sres_deviation_y");

            _ = migrationBuilder.AddColumn<double>(
                name: "sres_deviation_x",
                table: "session_result",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "sres_deviation_x",
                table: "session_result");

            _ = migrationBuilder.RenameColumn(
                name: "sres_math_exp_y",
                table: "session_result",
                newName: "sres_math_exp");

            _ = migrationBuilder.RenameColumn(
                name: "sres_math_exp_x",
                table: "session_result",
                newName: "sres_dispersion");

            _ = migrationBuilder.RenameColumn(
                name: "sres_deviation_y",
                table: "session_result",
                newName: "sres_deviation");
        }
    }
}
