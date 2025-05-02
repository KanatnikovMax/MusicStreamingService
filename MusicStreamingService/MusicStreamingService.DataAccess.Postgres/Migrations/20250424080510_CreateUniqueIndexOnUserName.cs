using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStreamingService.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class CreateUniqueIndexOnUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_UserName",
                table: "users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_UserName",
                table: "users");
        }
    }
}
