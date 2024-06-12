﻿// <auto-generated />
using System;
using DiscussionService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiscussionService.Migrations
{
    [DbContext(typeof(DiscussionDbContext))]
    [Migration("20240612113403_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DiscussionService.Models.DifficultyDiscussion", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<int>("Characteristic")
                        .HasColumnType("integer");

                    b.Property<int>("Difficulty")
                        .HasColumnType("integer");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("boolean");

                    b.Property<string>("MapDiscussionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MapDiscussionId", "Characteristic", "Difficulty")
                        .IsUnique();

                    b.ToTable("DifficultyDiscussions");
                });

            modelBuilder.Entity("DiscussionService.Models.MapDiscussion", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DiscussionOwnerIds")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MapsetId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Phase")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MapsetId")
                        .IsUnique();

                    b.ToTable("MapDiscussions");
                });

            modelBuilder.Entity("DiscussionService.Models.DifficultyDiscussion", b =>
                {
                    b.HasOne("DiscussionService.Models.MapDiscussion", null)
                        .WithMany("Discussions")
                        .HasForeignKey("MapDiscussionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscussionService.Models.MapDiscussion", b =>
                {
                    b.Navigation("Discussions");
                });
#pragma warning restore 612, 618
        }
    }
}