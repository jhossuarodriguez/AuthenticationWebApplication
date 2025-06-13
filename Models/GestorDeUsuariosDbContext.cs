using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationWebApplication.Models;

public partial class GestorDeUsuariosDbContext : DbContext
{
    public GestorDeUsuariosDbContext()
    {
    }

    public GestorDeUsuariosDbContext(DbContextOptions<GestorDeUsuariosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RevokedToken> RevokedTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=JHOSSUA-PC\\SQLEXPRESS; Database=GestorDeUsuariosDb; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RevokedToken>().ToTable("RevokedTokens");

            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC077748C3BC");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4CE5DAF8E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105349BBD7129").IsUnique();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsEmailConfirmed).HasColumnName("isEmailConfirmed");
            entity.Property(e => e.IsLockedOut).HasColumnName("isLockedOut");
            entity.Property(e => e.IsPhoneConfirmed).HasColumnName("isPhoneConfirmed");
            entity.Property(e => e.LastLoginAt).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
