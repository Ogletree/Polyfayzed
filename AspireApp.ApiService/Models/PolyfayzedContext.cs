﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService.Models;

public partial class PolyfayzedContext : DbContext
{
    public PolyfayzedContext(DbContextOptions<PolyfayzedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CursorState> CursorStates { get; set; }

    public virtual DbSet<Market> Markets { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Market>(entity =>
        {
            entity.ToTable("Market");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ConditionId).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Fpmm).IsRequired();
            entity.Property(e => e.MakerBaseFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MarketSlug).IsRequired();
            entity.Property(e => e.MinimumOrderSize).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinimumTickSize).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Question).IsRequired();
            entity.Property(e => e.QuestionId).IsRequired();
            entity.Property(e => e.TakerBaseFee).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");

            entity.HasIndex(e => e.MarketId, "IX_Tag_MarketId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).IsRequired();

            entity.HasOne(d => d.Market).WithMany(p => p.Tags).HasForeignKey(d => d.MarketId);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Team");

            entity.Property(e => e.Abbreviation)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.Alias).HasMaxLength(50);
            entity.Property(e => e.League)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Logo)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Record).HasMaxLength(10);
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Token");

            entity.HasIndex(e => e.MarketId, "IX_Token_MarketId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Outcome).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TokenId).IsRequired();

            entity.HasOne(d => d.Market).WithMany(p => p.Tokens).HasForeignKey(d => d.MarketId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}