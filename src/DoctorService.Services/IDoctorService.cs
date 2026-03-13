using DoctorService.InternalModels.DTOs;
using DoctorService.Utils.Common;

namespace DoctorService.Services;

public interface IDoctorService
{
    Task<ApiResponse<PagedResult<DoctorDto>>> GetDoctorsAsync(SearchQuery searchQuery);
    Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(int id);
    Task<ApiResponse<DoctorDto>> GetDoctorByDoctorIdAsync(string doctorId);
    Task<ApiResponse<DoctorDto>> GetDoctorByUserIdAsync(int userId);
    Task<ApiResponse<PagedResult<DoctorDto>>> GetDoctorsBySpecializationAsync(string specialization, int pageNumber, int pageSize);
    Task<ApiResponse<IEnumerable<string>>> GetSpecializationsAsync();
    Task<ApiResponse<string>> GenerateDoctorIdAsync();
    Task<ApiResponse<DoctorDto>> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
    Task<ApiResponse<DoctorDto>> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto);
    Task<ApiResponse<string>> DeleteDoctorAsync(int id);
}
