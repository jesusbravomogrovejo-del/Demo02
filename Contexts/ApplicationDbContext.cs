using System;
using System.Collections.Generic;
using Demo02.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo02.Contexts;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumno { get; set; }

    public virtual DbSet<Ciclo> Ciclo { get; set; }

    public virtual DbSet<Curso> Curso { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Beca).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Ciclo).WithMany(p => p.Alumno)
                .HasForeignKey(d => d.CicloId)
                .HasConstraintName("FK_Alumno_Ciclo");

            entity.HasOne(d => d.Curso).WithMany(p => p.Alumno)
                .HasForeignKey(d => d.CursoId)
                .HasConstraintName("FK_Alumno_Curso");
        });

        modelBuilder.Entity<Ciclo>(entity =>
        {
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.Property(e => e.Codigo)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
