using System;
using Microsoft.EntityFrameworkCore;

namespace Keeper.Core.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<NflPlayer> NflPlayers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=/Users/hanhossain/Developer/keeper/src/keeper.sqlite");
        }
    }
}
