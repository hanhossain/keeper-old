using Microsoft.EntityFrameworkCore.Migrations;

namespace Keeper.Core.Migrations
{
    public partial class AddDefensiveStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NflDefensiveStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    Sacks = table.Column<int>(type: "INTEGER", nullable: true),
                    Interceptions = table.Column<int>(type: "INTEGER", nullable: true),
                    FumblesRecovered = table.Column<int>(type: "INTEGER", nullable: true),
                    Safeties = table.Column<int>(type: "INTEGER", nullable: true),
                    Touchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    Def2PtRet = table.Column<int>(type: "INTEGER", nullable: true),
                    RetTouchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    PointsAllowed = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflDefensiveStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflDefensiveStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NflDefensiveStatistics");
        }
    }
}
