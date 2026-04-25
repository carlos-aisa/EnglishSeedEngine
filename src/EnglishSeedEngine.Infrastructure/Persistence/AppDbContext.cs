using EnglishSeedEngine.Domain.Students;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var student = modelBuilder.Entity<Student>();

        student.ToTable("students");
        student.HasKey(x => x.Id);
        student.Property(x => x.FullName).HasMaxLength(120).IsRequired();
        student.Property(x => x.Age).IsRequired();
        student.Property(x => x.TutorEmail).HasMaxLength(256).IsRequired();
        student.Property(x => x.TargetLevel).HasMaxLength(8).IsRequired();
        student.Property(x => x.CreatedAtUtc).IsRequired();
        student.HasIndex(x => x.TutorEmail).IsUnique();
    }
}

