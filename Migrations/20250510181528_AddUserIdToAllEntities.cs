using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Siniflar",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Ogrenciler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Oduncler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Kitaplar",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Kategoriler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Siniflar");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ogrenciler");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Oduncler");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Kitaplar");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Kategoriler");
        }
    }
}
