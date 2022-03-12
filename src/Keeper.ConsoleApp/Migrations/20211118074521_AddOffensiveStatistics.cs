using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keeper.Core.Migrations
{
    public partial class AddOffensiveStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NflOffensiveStatistics");
        }
    }
}
