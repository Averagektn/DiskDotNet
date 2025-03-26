using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class PrecisionToAccuracy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pit_precision",
                table: "path_in_target",
                newName: "pit_accuracy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pit_accuracy",
                table: "path_in_target",
                newName: "pit_precision");
        }
    }
}
