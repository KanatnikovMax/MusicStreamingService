using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SongsCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_artists_songs_artists_ArtistsId",
                table: "artists_songs");

            migrationBuilder.DropForeignKey(
                name: "FK_artists_songs_songs_SongsId",
                table: "artists_songs");

            migrationBuilder.RenameColumn(
                name: "SongsId",
                table: "artists_songs",
                newName: "SongId");

            migrationBuilder.RenameColumn(
                name: "ArtistsId",
                table: "artists_songs",
                newName: "ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_artists_songs_SongsId",
                table: "artists_songs",
                newName: "IX_artists_songs_SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_artists_songs_artists_ArtistId",
                table: "artists_songs",
                column: "ArtistId",
                principalTable: "artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_artists_songs_songs_SongId",
                table: "artists_songs",
                column: "SongId",
                principalTable: "songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_artists_songs_artists_ArtistId",
                table: "artists_songs");

            migrationBuilder.DropForeignKey(
                name: "FK_artists_songs_songs_SongId",
                table: "artists_songs");

            migrationBuilder.RenameColumn(
                name: "SongId",
                table: "artists_songs",
                newName: "SongsId");

            migrationBuilder.RenameColumn(
                name: "ArtistId",
                table: "artists_songs",
                newName: "ArtistsId");

            migrationBuilder.RenameIndex(
                name: "IX_artists_songs_SongId",
                table: "artists_songs",
                newName: "IX_artists_songs_SongsId");

            migrationBuilder.AddForeignKey(
                name: "FK_artists_songs_artists_ArtistsId",
                table: "artists_songs",
                column: "ArtistsId",
                principalTable: "artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_artists_songs_songs_SongsId",
                table: "artists_songs",
                column: "SongsId",
                principalTable: "songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
