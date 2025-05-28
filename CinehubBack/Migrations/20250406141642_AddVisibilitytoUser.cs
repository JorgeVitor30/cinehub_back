using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinehubBack.Migrations
{
    /// <inheritdoc />
    public partial class AddVisibilitytoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "visibility_public",
                table: "user",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "visibility_public",
                table: "user");
        }
    }
}
