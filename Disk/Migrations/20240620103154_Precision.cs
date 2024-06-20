using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class Precision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "pit_precision",
                table: "path_in_target",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pit_precision",
                table: "path_in_target");
        }
    }
}
