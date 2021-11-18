﻿using Microsoft.EntityFrameworkCore;

namespace Keeper.Core.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<NflPlayer> NflPlayers { get; set; }

        public DbSet<NflPlayerStatistics> NflPlayerStatistics { get; set; }

        public DbSet<NflKickingStatistics> NflKickingStatistics { get; set; }

        public DbSet<NflDefensiveStatistics> NflDefensiveStatistics { get; set; }

        public DbSet<NflOffensiveStatistics> NflOffensiveStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=/Users/hanhossain/Developer/keeper/src/keeper.sqlite");
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
