﻿// <auto-generated />
using System;
using Keeper.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Keeper.Core.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211103054156_AddDefensiveStatistics")]
    partial class AddDefensiveStatistics
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("Keeper.Core.Database.NflDefensiveStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Week")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Def2PtRet")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FumblesRecovered")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Interceptions")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PointsAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("RetTouchdowns")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Sacks")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Safeties")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Touchdowns")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflDefensiveStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflKickingStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Week")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FieldGoal0To19Yards")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FieldGoal20To29Yards")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FieldGoal30To39Yards")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FieldGoal40To49Yards")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FieldGoal50PlusYards")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PatMade")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflKickingStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Position")
                        .HasColumnType("TEXT");

                    b.Property<string>("Team")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("NflPlayers");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflPlayerStatistics", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Week")
                        .HasColumnType("INTEGER");

                    b.Property<double>("FantasyPoints")
                        .HasColumnType("REAL");

                    b.HasKey("PlayerId", "Season", "Week");

                    b.ToTable("NflPlayerStatistics");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflDefensiveStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.NflPlayer", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflKickingStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.NflPlayer", "Player")
                        .WithMany("NflKickingStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflPlayerStatistics", b =>
                {
                    b.HasOne("Keeper.Core.Database.NflPlayer", "Player")
                        .WithMany("PlayerStatistics")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Keeper.Core.Database.NflPlayer", b =>
                {
                    b.Navigation("NflKickingStatistics");

                    b.Navigation("PlayerStatistics");
                });
#pragma warning restore 612, 618
        }
    }
}
