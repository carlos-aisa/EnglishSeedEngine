using EnglishSeedEngine.Domain.Students;

namespace EnglishSeedEngine.Application.Students;

public sealed class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly TimeProvider _timeProvider;

    public StudentService(IStudentRepository studentRepository, TimeProvider timeProvider)
    {
        _studentRepository = studentRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Student> CreateAsync(CreateStudentInput input, CancellationToken cancellationToken)
    {
        var normalizedEmail = input.TutorEmail.Trim().ToLowerInvariant();
        var exists = await _studentRepository.TutorEmailExistsAsync(normalizedEmail, cancellationToken);

        if (exists)
        {
            throw new DuplicateTutorEmailException(normalizedEmail);
        }

        var student = Student.Create(
            input.FullName,
            input.Age,
            normalizedEmail,
            input.TargetLevel,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _studentRepository.AddAsync(student, cancellationToken);

        return student;
    }

    public Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _studentRepository.GetByIdAsync(id, cancellationToken);
    }
}

