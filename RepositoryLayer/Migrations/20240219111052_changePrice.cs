using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class changePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Stadiums",
                newName: "minPrice");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "maxPrice",
                table: "Stadiums",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Stadiums");

            migrationBuilder.DropColumn(
                name: "maxPrice",
                table: "Stadiums");

            migrationBuilder.RenameColumn(
                name: "minPrice",
                table: "Stadiums",
                newName: "Price");
        }
    }
}
