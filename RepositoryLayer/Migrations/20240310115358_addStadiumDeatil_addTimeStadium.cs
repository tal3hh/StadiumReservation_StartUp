using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class addStadiumDeatil_addTimeStadium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "View",
                table: "Stadiums");

            migrationBuilder.RenameColumn(
                name: "OpenTime",
                table: "Stadiums",
                newName: "openTime");

            migrationBuilder.RenameColumn(
                name: "OpenDay",
                table: "Stadiums",
                newName: "openDay");

            migrationBuilder.RenameColumn(
                name: "CloseTime",
                table: "Stadiums",
                newName: "closeTime");

            migrationBuilder.RenameColumn(
                name: "CloseDay",
                table: "Stadiums",
                newName: "closeDay");

            migrationBuilder.AlterColumn<string>(
                name: "openTime",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "closeTime",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "StadiumDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StadiumId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StadiumDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StadiumDetails_Stadiums_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeStadiums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    openTime = table.Column<int>(type: "int", nullable: false),
                    closeTime = table.Column<int>(type: "int", nullable: false),
                    nightTime = table.Column<int>(type: "int", nullable: false),
                    StadiumId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_StadiumDetails_StadiumId",
                table: "StadiumDetails",
                column: "StadiumId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeStadiums_StadiumId",
                table: "TimeStadiums",
                column: "StadiumId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StadiumDetails");

            migrationBuilder.DropTable(
                name: "TimeStadiums");

            migrationBuilder.RenameColumn(
                name: "openTime",
                table: "Stadiums",
                newName: "OpenTime");

            migrationBuilder.RenameColumn(
                name: "openDay",
                table: "Stadiums",
                newName: "OpenDay");

            migrationBuilder.RenameColumn(
                name: "closeTime",
                table: "Stadiums",
                newName: "CloseTime");

            migrationBuilder.RenameColumn(
                name: "closeDay",
                table: "Stadiums",
                newName: "CloseDay");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpenTime",
                table: "Stadiums",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CloseTime",
                table: "Stadiums",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "View",
                table: "Stadiums",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
