using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Models;

public partial class UniversityDbContext : DbContext
{
    public UniversityDbContext()
    {
    }

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseHasStudent> CourseHasStudents { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Secretary> Secretaries { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.IdCourse).HasName("PK__course__C18577573C205AEF");

            entity.ToTable("course");

            entity.Property(e => e.IdCourse).HasColumnName("idCOURSE");
            entity.Property(e => e.CourseSemester)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.ProfessorsAfm).HasColumnName("PROFESSORS_AFM");

            entity.HasOne(d => d.ProfessorsAfmNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.ProfessorsAfm)
                .HasConstraintName("FK_Course_Professors");
        });

        modelBuilder.Entity<CourseHasStudent>(entity =>
        {
            entity.HasKey(e => new { e.CourseIdCourse, e.StudentsRegistrationNumber }).HasName("PK__course_h__A12B81E5424DD8EB");

            entity.ToTable("course_has_students");

            entity.Property(e => e.CourseIdCourse).HasColumnName("COURSE_idCOURSE");
            entity.Property(e => e.StudentsRegistrationNumber).HasColumnName("STUDENTS_RegistrationNumber");

            entity.HasOne(d => d.CourseIdCourseNavigation).WithMany(p => p.CourseHasStudents)
                .HasForeignKey(d => d.CourseIdCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChS_Course");

            entity.HasOne(d => d.StudentsRegistrationNumberNavigation).WithMany(p => p.CourseHasStudents)
                .HasForeignKey(d => d.StudentsRegistrationNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChS_Students");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Afm).HasName("PK__professo__C6906E6349CE852B");

            entity.ToTable("professors");

            entity.Property(e => e.Afm)
                .ValueGeneratedNever()
                .HasColumnName("AFM");
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Professors)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Professors_Users");
        });

        modelBuilder.Entity<Secretary>(entity =>
        {
            entity.HasKey(e => e.Phonenumber).HasName("PK__secretar__9FDCA5A63D2FD668");

            entity.ToTable("secretaries");

            entity.Property(e => e.Phonenumber).ValueGeneratedNever();
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Secretaries)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Secretaries_Users");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.RegistrationNumber).HasName("PK__students__E88646037562A57C");

            entity.ToTable("students");

            entity.Property(e => e.RegistrationNumber).ValueGeneratedNever();
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__users__F3DBC57319394AD1");

            entity.ToTable("users");

            entity.Property(e => e.Username)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
