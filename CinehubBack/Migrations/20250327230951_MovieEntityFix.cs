using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinehubBack.Migrations
{
    /// <inheritdoc />
    public partial class MovieEntityFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BackPhotoUrl",
                table: "movie",
                newName: "poster_path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "poster_path",
                table: "movie",
                newName: "BackPhotoUrl");
        }
    }
}
