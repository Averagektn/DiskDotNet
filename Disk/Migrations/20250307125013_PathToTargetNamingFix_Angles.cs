﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disk.Migrations
{
    /// <inheritdoc />
    public partial class PathToTargetNamingFix_Angles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ptt_angle_speed",
                table: "path_to_target",
                newName: "ptt_distance");

            migrationBuilder.RenameColumn(
                name: "ptt_ange_distance",
                table: "path_to_target",
                newName: "ptt_average_speed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ptt_distance",
                table: "path_to_target",
                newName: "ptt_angle_speed");

            migrationBuilder.RenameColumn(
                name: "ptt_average_speed",
                table: "path_to_target",
                newName: "ptt_ange_distance");
        }
    }
}
