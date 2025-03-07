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
            migrationBuilder.RenameColumn(
                name: "sres_math_exp",
                table: "session_result",
                newName: "sres_math_exp_y");

            migrationBuilder.RenameColumn(
                name: "sres_dispersion",
                table: "session_result",
                newName: "sres_math_exp_x");

            migrationBuilder.RenameColumn(
                name: "sres_deviation",
                table: "session_result",
                newName: "sres_deviation_y");

            migrationBuilder.AddColumn<double>(
                name: "sres_deviation_x",
                table: "session_result",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sres_deviation_x",
                table: "session_result");

            migrationBuilder.RenameColumn(
                name: "sres_math_exp_y",
                table: "session_result",
                newName: "sres_math_exp");

            migrationBuilder.RenameColumn(
                name: "sres_math_exp_x",
                table: "session_result",
                newName: "sres_dispersion");

            migrationBuilder.RenameColumn(
                name: "sres_deviation_y",
                table: "session_result",
                newName: "sres_deviation");
        }
    }
}
