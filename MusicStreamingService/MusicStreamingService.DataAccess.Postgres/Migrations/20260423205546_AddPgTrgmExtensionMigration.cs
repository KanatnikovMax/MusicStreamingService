using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddPgTrgmExtensionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            migrationBuilder.DropIndex(
                name: "IX_songs_Title",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "IX_artists_Name",
                table: "artists");

            migrationBuilder.DropIndex(
                name: "IX_albums_Title",
                table: "albums");

            migrationBuilder.CreateIndex(
                name: "ix_songs_title_trgm",
                table: "songs",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_playlists_name_trgm",
                table: "playlists",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_artists_name_trgm",
                table: "artists",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_albums_title_trgm",
                table: "albums",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS pg_trgm;");

            migrationBuilder.DropIndex(
                name: "ix_songs_title_trgm",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_playlists_name_trgm",
                table: "playlists");

            migrationBuilder.DropIndex(
                name: "ix_artists_name_trgm",
                table: "artists");

            migrationBuilder.DropIndex(
                name: "ix_albums_title_trgm",
                table: "albums");

            migrationBuilder.CreateIndex(
                name: "IX_songs_Title",
                table: "songs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_artists_Name",
                table: "artists",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_albums_Title",
                table: "albums",
                column: "Title");
        }
    }
}
