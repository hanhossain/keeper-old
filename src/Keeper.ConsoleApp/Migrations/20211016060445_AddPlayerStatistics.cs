using Microsoft.EntityFrameworkCore.Migrations;

namespace Keeper.Core.Migrations
{
    public partial class AddPlayerStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NflPlayerStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    FantasyPoints = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflPlayerStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflPlayerStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NflPlayerStatistics");
        }
    }
}
