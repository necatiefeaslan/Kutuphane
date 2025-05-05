using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class AddOdunc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Oduncler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KitapId = table.Column<int>(type: "INTEGER", nullable: false),
                    OgrenciId = table.Column<int>(type: "INTEGER", nullable: false),
                    OduncAlmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IadeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IadeEdildi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oduncler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oduncler_Kitaplar_KitapId",
                        column: x => x.KitapId,
                        principalTable: "Kitaplar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Oduncler_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oduncler_KitapId",
                table: "Oduncler",
                column: "KitapId");

            migrationBuilder.CreateIndex(
                name: "IX_Oduncler_OgrenciId",
                table: "Oduncler",
                column: "OgrenciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Oduncler");
        }
    }
}
