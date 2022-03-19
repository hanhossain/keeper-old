using System;
using System.IO;
using Keeper.Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Keeper.Core.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<SleeperPlayer> SleeperPlayers { get; set; }
        
        public DbSet<NflPlayer> NflPlayers { get; set; }

        public DbSet<NflPlayerStatistics> NflPlayerStatistics { get; set; }

        public DbSet<NflKickingStatistics> NflKickingStatistics { get; set; }

        public DbSet<NflDefensiveStatistics> NflDefensiveStatistics { get; set; }

        public DbSet<NflOffensiveStatistics> NflOffensiveStatistics { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // needed for ios
            SQLitePCL.Batteries_V2.Init();

            // found path from:
            // https://stackoverflow.com/questions/47237414/what-is-the-best-environment-specialfolder-for-store-application-data-in-xamarin
            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "..",
                "Library",
                "database.db");

            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NflPlayerStatistics>()
                .HasKey(x => new { x.PlayerId, x.Season, x.Week });

            modelBuilder.Entity<NflKickingStatistics>()
                .HasKey(x => new { x.PlayerId, x.Season, x.Week });

            modelBuilder.Entity<NflDefensiveStatistics>()
                .HasKey(x => new { x.PlayerId, x.Season, x.Week });

            modelBuilder.Entity<NflOffensiveStatistics>()
                .HasKey(x => new { x.PlayerId, x.Season, x.Week });
        }
    }
}
