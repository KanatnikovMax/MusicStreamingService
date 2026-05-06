using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MusicStreamingService.DataAccess.Postgres.Context;

#nullable disable

namespace MusicStreamingService.DataAccess.Postgres.Migrations
{
    [DbContext(typeof(MusicServiceDbContext))]
    [Migration("20260506120000_AddPlayCounts")]
    public partial class AddPlayCounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PlayCount",
                table: "songs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PlayCount",
                table: "artists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_songs_PlayCount_CreatedAt",
                table: "songs",
                columns: new[] { "PlayCount", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_artists_PlayCount_CreatedAt",
                table: "artists",
                columns: new[] { "PlayCount", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_songs_PlayCount_CreatedAt",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "IX_artists_PlayCount_CreatedAt",
                table: "artists");

            migrationBuilder.DropColumn(
                name: "PlayCount",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "PlayCount",
                table: "artists");
        }
    }
}
