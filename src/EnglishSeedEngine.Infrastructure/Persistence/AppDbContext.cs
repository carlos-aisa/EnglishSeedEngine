using EnglishSeedEngine.Domain.LearningPlans;
using EnglishSeedEngine.Domain.Lessons;
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

    public DbSet<LearningPlan> LearningPlans => Set<LearningPlan>();

    public DbSet<Lesson> Lessons => Set<Lesson>();

    public DbSet<LessonMaterial> LessonMaterials => Set<LessonMaterial>();

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
        student.Property(x => x.InitialAssessmentLevel).HasMaxLength(8);
        student.HasIndex(x => x.TutorEmail).IsUnique();

        var learningPlan = modelBuilder.Entity<LearningPlan>();
        learningPlan.ToTable("learning_plans");
        learningPlan.HasKey(x => x.Id);
        learningPlan.Property(x => x.StudentId).IsRequired();
        learningPlan.Property(x => x.StartLevel).HasMaxLength(8).IsRequired();
        learningPlan.Property(x => x.TargetLevel).HasMaxLength(8).IsRequired();
        learningPlan.Property(x => x.Status).HasMaxLength(24).IsRequired();
        learningPlan.Property(x => x.CreatedAtUtc).IsRequired();
        learningPlan.HasIndex(x => x.StudentId);
        learningPlan.HasMany(x => x.WeeklyGoals)
            .WithOne()
            .HasForeignKey(x => x.LearningPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        var weeklyGoal = modelBuilder.Entity<LearningPlanWeeklyGoal>();
        weeklyGoal.ToTable("learning_plan_weekly_goals");
        weeklyGoal.HasKey(x => x.Id);
        weeklyGoal.Property(x => x.WeekNumber).IsRequired();
        weeklyGoal.Property(x => x.Goal).HasMaxLength(300).IsRequired();

        var lesson = modelBuilder.Entity<Lesson>();
        lesson.ToTable("lessons");
        lesson.HasKey(x => x.Id);
        lesson.Property(x => x.LearningPlanId).IsRequired();
        lesson.Property(x => x.WeekNumber).IsRequired();
        lesson.Property(x => x.WeeklyFocus).HasMaxLength(300).IsRequired();
        lesson.Property(x => x.TargetDifficulty).HasMaxLength(24).IsRequired();
        lesson.Property(x => x.Status).HasMaxLength(24).IsRequired();
        lesson.Property(x => x.CreatedAtUtc).IsRequired();
        lesson.HasIndex(x => x.LearningPlanId);

        var lessonMaterial = modelBuilder.Entity<LessonMaterial>();
        lessonMaterial.ToTable("lesson_materials");
        lessonMaterial.HasKey(x => x.Id);
        lessonMaterial.Property(x => x.LessonId).IsRequired();
        lessonMaterial.Property(x => x.Version).IsRequired();
        lessonMaterial.Property(x => x.VocabularyJson).HasColumnType("jsonb").IsRequired();
        lessonMaterial.Property(x => x.PhrasesJson).HasColumnType("jsonb").IsRequired();
        lessonMaterial.Property(x => x.ExercisesJson).HasColumnType("jsonb").IsRequired();
        lessonMaterial.Property(x => x.GeneratedAtUtc).IsRequired();
        lessonMaterial.HasIndex(x => x.LessonId);
        lessonMaterial.HasIndex(x => new { x.LessonId, x.Version }).IsUnique();
    }
}
