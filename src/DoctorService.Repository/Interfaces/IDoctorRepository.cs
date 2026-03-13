using DoctorService.InternalModels.Entities;
using DoctorService.Utils.Common;

namespace DoctorService.Repository;

public interface IDoctorRepository
{
    Task<PagedResult<DoctorEntity>> GetDoctorsAsync(SearchQuery searchQuery);
    Task<DoctorEntity?> GetDoctorByIdAsync(int id);
    Task<DoctorEntity?> GetDoctorByDoctorIdAsync(string doctorId);
    Task<DoctorEntity?> GetDoctorByUserIdAsync(int userId);
    Task<PagedResult<DoctorEntity>> GetDoctorsBySpecializationAsync(string specialization, int pageNumber, int pageSize);
    Task<IReadOnlyCollection<string>> GetSpecializationsAsync();
    Task<string> GenerateDoctorIdAsync();
    Task<DoctorEntity> CreateDoctorAsync(DoctorEntity doctor);
    Task<DoctorEntity?> UpdateDoctorAsync(int id, DoctorEntity doctor);
    Task<bool> DeleteDoctorAsync(int id);
}

