﻿// <auto-generated />
using System;
using AspireApp.ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AspireApp.ApiService.Migrations
{
    [DbContext(typeof(PolyfayzedContext))]
    [Migration("20250102025044_Null")]
    partial class Null
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AspireApp.ApiService.Models.Market", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime?>("AcceptingOrderTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("AcceptingOrders")
                        .HasColumnType("bit");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<bool>("Archived")
                        .HasColumnType("bit");

                    b.Property<bool>("Closed")
                        .HasColumnType("bit");

                    b.Property<string>("ConditionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EnableOrderBook")
                        .HasColumnType("bit");

                    b.Property<DateTime>("EndDateIso")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fpmm")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("GameStartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Is5050Outcome")
                        .HasColumnType("bit");

                    b.Property<decimal>("MakerBaseFee")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("MarketSlug")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MinimumOrderSize")
                        .HasColumnType("int");

                    b.Property<decimal>("MinimumTickSize")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<bool>("NegRisk")
                        .HasColumnType("bit");

                    b.Property<string>("NegRiskMarketId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NegRiskRequestId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NotificationsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecondsDelay")
                        .HasColumnType("int");

                    b.Property<decimal>("TakerBaseFee")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.ToTable("Market", (string)null);
                });

            modelBuilder.Entity("AspireApp.ApiService.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<Guid>("MarketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "MarketId" }, "IX_Tag_MarketId");

                    b.ToTable("Tag", (string)null);
                });

            modelBuilder.Entity("AspireApp.ApiService.Models.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<Guid>("MarketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Outcome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("TokenId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Winner")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "MarketId" }, "IX_Token_MarketId");

                    b.ToTable("Token", (string)null);
                });

            modelBuilder.Entity("AspireApp.ApiService.Models.Tag", b =>
                {
                    b.HasOne("AspireApp.ApiService.Models.Market", "Market")
                        .WithMany("Tags")
                        .HasForeignKey("MarketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Market");
                });

            modelBuilder.Entity("AspireApp.ApiService.Models.Token", b =>
                {
                    b.HasOne("AspireApp.ApiService.Models.Market", "Market")
                        .WithMany("Tokens")
                        .HasForeignKey("MarketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Market");
                });

            modelBuilder.Entity("AspireApp.ApiService.Models.Market", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
