﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModCore.Common.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    [DbContext(typeof(DatabaseConnection))]
    [Migration("20240107195951_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseGuild", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("LoggingChannel")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("logging_channel_id");

                    b.HasKey("Id");

                    b.ToTable("mcore_guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseLevelData", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<long>("Experience")
                        .HasColumnType("bigint")
                        .HasColumnName("experience");

                    b.Property<DateTimeOffset>("LastGrant")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_xp_grant");

                    b.HasKey("GuildId", "UserId");

                    b.ToTable("mcore_leveldata");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseLevelData", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("LevelData")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseGuild", b =>
                {
                    b.Navigation("LevelData");
                });
#pragma warning restore 612, 618
        }
    }
}
