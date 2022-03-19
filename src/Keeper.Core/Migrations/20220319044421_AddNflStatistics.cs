using Microsoft.EntityFrameworkCore.Migrations;

namespace Keeper.Core.Migrations
{
    public partial class AddNflStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NflPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    Team = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflPlayers", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "NflKickingStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    PatMade = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal0To19Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal20To29Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal30To39Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal40To49Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal50PlusYards = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflKickingStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflKickingStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NflOffensiveStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    PassingYards = table.Column<int>(type: "INTEGER", nullable: true),
                    PassingTouchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    PassingInterceptions = table.Column<int>(type: "INTEGER", nullable: true),
                    RushingYards = table.Column<int>(type: "INTEGER", nullable: true),
                    RushingTouchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    ReceivingReceptions = table.Column<int>(type: "INTEGER", nullable: true),
                    ReceivingYards = table.Column<int>(type: "INTEGER", nullable: true),
                    ReceivingTouchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    ReturningTouchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    TwoPointConversions = table.Column<int>(type: "INTEGER", nullable: true),
                    FumbleTouchdowns = table.Column<int>(type: "INTEGER", nullable: true),
                    FumblesLost = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflOffensiveStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflOffensiveStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "NflDefensiveStatistics");

            migrationBuilder.DropTable(
                name: "NflKickingStatistics");

            migrationBuilder.DropTable(
                name: "NflOffensiveStatistics");

            migrationBuilder.DropTable(
                name: "NflPlayerStatistics");

            migrationBuilder.DropTable(
                name: "NflPlayers");
        }
    }
}
