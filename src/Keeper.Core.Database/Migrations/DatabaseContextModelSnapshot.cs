﻿// <auto-generated />
using System;
using Keeper.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Keeper.Core.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Keeper.Core.Database.Models.NflDefensiveStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("Season")
                        .HasColumnType("int");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<int?>("Def2PtRet")
                        .HasColumnType("int");

                    b.Property<int?>("FumblesRecovered")
                        .HasColumnType("int");

                    b.Property<int?>("Interceptions")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed")
                        .HasColumnType("int");

                    b.Property<int?>("RetTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("Sacks")
                        .HasColumnType("int");

                    b.Property<int?>("Safeties")
                        .HasColumnType("int");

                    b.Property<int?>("Touchdowns")
                        .HasColumnType("int");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflDefensiveStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflKickingStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("Season")
                        .HasColumnType("int");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal0To19Yards")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal20To29Yards")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal30To39Yards")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal40To49Yards")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal50PlusYards")
                        .HasColumnType("int");

                    b.Property<int?>("PatMade")
                        .HasColumnType("int");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflKickingStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflOffensiveStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("Season")
                        .HasColumnType("int");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<int?>("FumbleTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("FumblesLost")
                        .HasColumnType("int");

                    b.Property<int?>("PassingInterceptions")
                        .HasColumnType("int");

                    b.Property<int?>("PassingTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("PassingYards")
                        .HasColumnType("int");

                    b.Property<int?>("ReceivingReceptions")
                        .HasColumnType("int");

                    b.Property<int?>("ReceivingTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("ReceivingYards")
                        .HasColumnType("int");

                    b.Property<int?>("ReturningTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("RushingTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("RushingYards")
                        .HasColumnType("int");

                    b.Property<int?>("TwoPointConversions")
                        .HasColumnType("int");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflOffensiveStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflPlayer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Team")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NflPlayers");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflPlayerStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("Season")
                        .HasColumnType("int");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<double>("FantasyPoints")
                        .HasColumnType("float");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflPlayerStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.SleeperPlayer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NflId")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Team")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NflId")
                        .IsUnique()
                        .HasFilter("[NflId] IS NOT NULL");

                    b.ToTable("SleeperPlayers");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflDefensiveStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.Models.NflPlayer", "Player")
                        .WithMany("NflDefensiveStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflKickingStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.Models.NflPlayer", "Player")
                        .WithMany("NflKickingStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflOffensiveStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.Models.NflPlayer", "Player")
                        .WithMany("NflOffensiveStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflPlayerStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.Models.NflPlayer", "Player")
                        .WithMany("PlayerStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.SleeperPlayer", b =>
                {
                    b.HasOne("Keeper.Core.Database.Models.NflPlayer", "NflPlayer")
                        .WithOne("SleeperPlayer")
                        .HasForeignKey("Keeper.Core.Database.Models.SleeperPlayer", "NflId");

                    b.Navigation("NflPlayer");
                });

            modelBuilder.Entity("Keeper.Core.Database.Models.NflPlayer", b =>
                {
                    b.Navigation("NflDefensiveStatistics");

                    b.Navigation("NflKickingStatistics");

                    b.Navigation("NflOffensiveStatistics");

                    b.Navigation("PlayerStatistics");

                    b.Navigation("SleeperPlayer");
                });
#pragma warning restore 612, 618
        }
    }
}
