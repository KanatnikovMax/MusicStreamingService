using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAlbumPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "albums",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "albums");
        }
    }
}
