using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class AppUser_Stadium_connection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Stadiums",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stadiums_AppUserId",
                table: "Stadiums",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Stadiums_AspNetUsers_AppUserId",
                table: "Stadiums",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stadiums_AspNetUsers_AppUserId",
                table: "Stadiums");

            migrationBuilder.DropIndex(
                name: "IX_Stadiums_AppUserId",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Stadiums");
        }
    }
}
