using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class RenamedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.RenameIndex(
                name: "IX_nsp",
                table: "patient",
                newName: "IX_Patient_name_surname_patronymic");

            _ = migrationBuilder.RenameIndex(
                name: "IX_UNQ_map_map_name",
                table: "map",
                newName: "IX_UNQ_Map_name");

            _ = migrationBuilder.RenameIndex(
                name: "IX_UNQ_attempt_att_log_file_path",
                table: "attempt",
                newName: "IX_UNQ_Attempt_log_file_path");

            _ = migrationBuilder.AlterColumn<string>(
                name: "map_description",
                table: "map",
                type: "TEXT",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.RenameIndex(
                name: "IX_Patient_name_surname_patronymic",
                table: "patient",
                newName: "IX_nsp");

            _ = migrationBuilder.RenameIndex(
                name: "IX_UNQ_Map_name",
                table: "map",
                newName: "IX_UNQ_map_map_name");

            _ = migrationBuilder.RenameIndex(
                name: "IX_UNQ_Attempt_log_file_path",
                table: "attempt",
                newName: "IX_UNQ_attempt_att_log_file_path");

            _ = migrationBuilder.AlterColumn<string>(
                name: "map_description",
                table: "map",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValue: "");
        }
    }
}
