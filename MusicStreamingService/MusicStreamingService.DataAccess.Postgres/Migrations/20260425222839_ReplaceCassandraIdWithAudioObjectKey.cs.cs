using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCassandraIdWithAudioObjectKeycs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CassandraId",
                table: "songs");

            migrationBuilder.AddColumn<string>(
                name: "AudioObjectKey",
                table: "songs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioObjectKey",
                table: "songs");

            migrationBuilder.AddColumn<Guid>(
                name: "CassandraId",
                table: "songs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
