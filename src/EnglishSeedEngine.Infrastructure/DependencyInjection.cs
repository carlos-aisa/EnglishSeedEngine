using EnglishSeedEngine.Application.LearningPlans;
using EnglishSeedEngine.Application.Lessons;
using EnglishSeedEngine.Application.Students;
using EnglishSeedEngine.Infrastructure.Persistence;
using EnglishSeedEngine.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishSeedEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ILearningPlanRepository, LearningPlanRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();

        return services;
    }
}
