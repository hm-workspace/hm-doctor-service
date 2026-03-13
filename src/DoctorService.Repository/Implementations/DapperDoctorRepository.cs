using Dapper;
using DoctorService.Data;
using DoctorService.InternalModels.Entities;
using DoctorService.Utils.Common;

namespace DoctorService.Repository;

public class DapperDoctorRepository : IDoctorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperDoctorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<PagedResult<DoctorEntity>> GetDoctorsAsync(SearchQuery searchQuery)
    {
        var query = DoctorInMemoryStore.Doctors.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchQuery.SearchTerm))
        {
            query = query.Where(x =>
                x.DoctorId.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.FirstName.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.LastName.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.Specialization.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var total = query.Count();
        var items = query.OrderBy(x => x.Id)
            .Skip((searchQuery.PageNumber - 1) * searchQuery.PageSize)
            .Take(searchQuery.PageSize)
            .ToList();
        return Task.FromResult(new PagedResult<DoctorEntity>(items, total, searchQuery.PageNumber, searchQuery.PageSize));
    }

    public Task<DoctorEntity?> GetDoctorByIdAsync(int id) =>
        Task.FromResult(DoctorInMemoryStore.Doctors.FirstOrDefault(x => x.Id == id));

    public Task<DoctorEntity?> GetDoctorByDoctorIdAsync(string doctorId) =>
        Task.FromResult(DoctorInMemoryStore.Doctors.FirstOrDefault(x => x.DoctorId.Equals(doctorId, StringComparison.OrdinalIgnoreCase)));

    public Task<DoctorEntity?> GetDoctorByUserIdAsync(int userId) =>
        Task.FromResult(DoctorInMemoryStore.Doctors.FirstOrDefault(x => x.UserId == userId));

    public Task<PagedResult<DoctorEntity>> GetDoctorsBySpecializationAsync(string specialization, int pageNumber, int pageSize)
    {
        var query = DoctorInMemoryStore.Doctors.Where(x => x.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase));
        var total = query.Count();
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<DoctorEntity>(items, total, pageNumber, pageSize));
    }

    public Task<IReadOnlyCollection<string>> GetSpecializationsAsync() =>
        Task.FromResult<IReadOnlyCollection<string>>(DoctorInMemoryStore.Doctors
            .Select(x => x.Specialization)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList());

    public Task<string> GenerateDoctorIdAsync()
    {
        var next = DoctorInMemoryStore.Doctors.Count + 1;
        return Task.FromResult($"DOC{next:000}");
    }

    public Task<DoctorEntity> CreateDoctorAsync(DoctorEntity doctor)
    {
        doctor.Id = Interlocked.Increment(ref DoctorInMemoryStore.DoctorSeed);
        DoctorInMemoryStore.Doctors.Add(doctor);
        return Task.FromResult(doctor);
    }

    public Task<DoctorEntity?> UpdateDoctorAsync(int id, DoctorEntity doctor)
    {
        var existing = DoctorInMemoryStore.Doctors.FirstOrDefault(x => x.Id == id);
        if (existing is null)
        {
            return Task.FromResult<DoctorEntity?>(null);
        }

        existing.FirstName = doctor.FirstName;
        existing.LastName = doctor.LastName;
        existing.Specialization = doctor.Specialization;
        existing.Email = doctor.Email;
        existing.Phone = doctor.Phone;
        existing.YearsOfExperience = doctor.YearsOfExperience;
        existing.IsActive = doctor.IsActive;
        return Task.FromResult<DoctorEntity?>(existing);
    }

    public Task<bool> DeleteDoctorAsync(int id)
    {
        var existing = DoctorInMemoryStore.Doctors.FirstOrDefault(x => x.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        DoctorInMemoryStore.Doctors.Remove(existing);
        return Task.FromResult(true);
    }
}

internal static class DoctorInMemoryStore
{
    public static int DoctorSeed = 1;

    public static readonly List<DoctorEntity> Doctors =
    [
        new DoctorEntity
        {
            Id = 1,
            DoctorId = "DOC001",
            UserId = 1001,
            FirstName = "Kiran",
            LastName = "Rao",
            Specialization = "Cardiology",
            Email = "kiran.rao@hm.local",
            Phone = "9000001001",
            YearsOfExperience = 8,
            IsActive = true
        }
    ];
}
