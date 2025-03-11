using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniquePatientPhones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UNQ_phone_home",
                table: "patient");

            migrationBuilder.DropIndex(
                name: "IX_UNQ_phone_mobile",
                table: "patient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UNQ_phone_home",
                table: "patient",
                column: "pat_phone_home",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UNQ_phone_mobile",
                table: "patient",
                column: "pat_phone_mobile",
                unique: true);
        }
    }
}
