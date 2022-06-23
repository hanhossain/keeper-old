using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keeper.Core.Database.Migrations
{
    public partial class AddPlayerLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NflId",
                table: "SleeperPlayers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SleeperPlayers_NflId",
                table: "SleeperPlayers",
                column: "NflId",
                unique: true,
                filter: "[NflId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SleeperPlayers_NflPlayers_NflId",
                table: "SleeperPlayers",
                column: "NflId",
                principalTable: "NflPlayers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SleeperPlayers_NflPlayers_NflId",
                table: "SleeperPlayers");

            migrationBuilder.DropIndex(
                name: "IX_SleeperPlayers_NflId",
                table: "SleeperPlayers");

            migrationBuilder.DropColumn(
                name: "NflId",
                table: "SleeperPlayers");
        }
    }
}
