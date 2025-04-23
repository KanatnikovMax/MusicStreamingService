using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "timezone('utc', now())");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "artists",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "timezone('utc', now())");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "albums",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "timezone('utc', now())");

            migrationBuilder.CreateIndex(
                name: "IX_songs_CreatedAt",
                table: "songs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_artists_CreatedAt",
                table: "artists",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_albums_CreatedAt",
                table: "albums",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_songs_CreatedAt",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "IX_artists_CreatedAt",
                table: "artists");

            migrationBuilder.DropIndex(
                name: "IX_albums_CreatedAt",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "artists");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "albums");
        }
    }
}
