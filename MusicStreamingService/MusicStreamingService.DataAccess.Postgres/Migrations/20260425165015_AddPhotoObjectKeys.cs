using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoObjectKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "playlists");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "artists");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "albums");

            migrationBuilder.AddColumn<string>(
                name: "PhotoObjectKey",
                table: "playlists",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoObjectKey",
                table: "artists",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoObjectKey",
                table: "albums",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoObjectKey",
                table: "playlists");

            migrationBuilder.DropColumn(
                name: "PhotoObjectKey",
                table: "artists");

            migrationBuilder.DropColumn(
                name: "PhotoObjectKey",
                table: "albums");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "playlists",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "artists",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "albums",
                type: "bytea",
                nullable: true);
        }
    }
}
