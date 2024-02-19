using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class addOpenStadiumClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseDay",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "OpenDay",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Stadiums");

            migrationBuilder.AddColumn<int>(
                name: "lengthtSize",
                table: "Stadiums",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "widthSize",
                table: "Stadiums",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OpenStadiums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpenDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StadiumId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenStadiums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenStadiums_Stadiums_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenStadiums_StadiumId",
                table: "OpenStadiums",
                column: "StadiumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenStadiums");

            migrationBuilder.DropColumn(
                name: "lengthtSize",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "widthSize",
                table: "Stadiums");

            migrationBuilder.AddColumn<string>(
                name: "CloseDay",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloseTime",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenDay",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTime",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
