using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class addAREA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Stadiums_StadiumId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "lengthtSize",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "widthSize",
                table: "Stadiums");

            migrationBuilder.RenameColumn(
                name: "StadiumId",
                table: "Reservations",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_StadiumId",
                table: "Reservations",
                newName: "IX_Reservations_AreaId");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    widthSize = table.Column<int>(type: "int", nullable: false),
                    lengthtSize = table.Column<int>(type: "int", nullable: false),
                    StadiumId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_Stadiums_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_StadiumId",
                table: "Areas",
                column: "StadiumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Areas_AreaId",
                table: "Reservations",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Areas_AreaId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Reservations",
                newName: "StadiumId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_AreaId",
                table: "Reservations",
                newName: "IX_Reservations_StadiumId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Stadiums_StadiumId",
                table: "Reservations",
                column: "StadiumId",
                principalTable: "Stadiums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
