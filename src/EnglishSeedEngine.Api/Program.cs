using EnglishSeedEngine.Application.LearningPlans;
using EnglishSeedEngine.Application.LessonMaterials;
using EnglishSeedEngine.Application.Lessons;
using EnglishSeedEngine.Application.PracticeSessions;
using EnglishSeedEngine.Application.Students;
using EnglishSeedEngine.Infrastructure;
using EnglishSeedEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ILearningPlanService, LearningPlanService>();
builder.Services.AddScoped<ILessonMaterialService, LessonMaterialService>();
builder.Services.AddScoped<IParentFeedbackService, ParentFeedbackService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IPracticeSessionService, PracticeSessionService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentProgressService, StudentProgressService>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();

await EnsureDatabaseCreatedAsync(app.Services);

app.Run();

static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

public partial class Program;
