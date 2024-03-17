using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class updateStadiumClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeStadiums");

            migrationBuilder.DropColumn(
                name: "closeDay",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "closeTime",
                table: "Stadiums");

            migrationBuilder.RenameColumn(
                name: "openTime",
                table: "Stadiums",
                newName: "OpenCloseHour");

            migrationBuilder.RenameColumn(
                name: "openDay",
                table: "Stadiums",
                newName: "OpenCloseDay");

            migrationBuilder.AddColumn<int>(
                name: "closeHour",
                table: "Stadiums",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nightHour",
                table: "Stadiums",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "openHour",
                table: "Stadiums",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closeHour",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "nightHour",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "openHour",
                table: "Stadiums");

            migrationBuilder.RenameColumn(
                name: "OpenCloseHour",
                table: "Stadiums",
                newName: "openTime");

            migrationBuilder.RenameColumn(
                name: "OpenCloseDay",
                table: "Stadiums",
                newName: "openDay");

            migrationBuilder.AddColumn<string>(
                name: "closeDay",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "closeTime",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeStadiums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StadiumId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    closeTime = table.Column<int>(type: "int", nullable: false),
                    nightTime = table.Column<int>(type: "int", nullable: false),
                    openTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeStadiums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeStadiums_Stadiums_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeStadiums_StadiumId",
                table: "TimeStadiums",
                column: "StadiumId",
                unique: true);
        }
    }
}
