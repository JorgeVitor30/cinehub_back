using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinehubBack.Migrations
{
    /// <inheritdoc />
    public partial class UserPhotoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "photo",
                table: "user",
                type: "longblob",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photo",
                table: "user");
        }
    }
}
