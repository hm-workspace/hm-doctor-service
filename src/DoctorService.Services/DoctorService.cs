using DoctorService.InternalModels.DTOs;
using DoctorService.InternalModels.Entities;
using DoctorService.Repository;
using DoctorService.Utils.Common;

namespace DoctorService.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<ApiResponse<PagedResult<DoctorDto>>> GetDoctorsAsync(SearchQuery searchQuery)
    {
        var page = await _doctorRepository.GetDoctorsAsync(searchQuery);
        var dto = new PagedResult<DoctorDto>(page.Items.Select(DoctorDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<DoctorDto>>.Ok(dto);
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(int id)
    {
        var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
        return doctor is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(doctor));
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByDoctorIdAsync(string doctorId)
    {
        var doctor = await _doctorRepository.GetDoctorByDoctorIdAsync(doctorId);
        return doctor is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(doctor));
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByUserIdAsync(int userId)
    {
        var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
        return doctor is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(doctor));
    }

    public async Task<ApiResponse<PagedResult<DoctorDto>>> GetDoctorsBySpecializationAsync(string specialization, int pageNumber, int pageSize)
    {
        var page = await _doctorRepository.GetDoctorsBySpecializationAsync(specialization, pageNumber, pageSize);
        var dto = new PagedResult<DoctorDto>(page.Items.Select(DoctorDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<DoctorDto>>.Ok(dto);
    }

    public async Task<ApiResponse<IEnumerable<string>>> GetSpecializationsAsync()
    {
        return ApiResponse<IEnumerable<string>>.Ok(await _doctorRepository.GetSpecializationsAsync());
    }

    public async Task<ApiResponse<string>> GenerateDoctorIdAsync()
    {
        return ApiResponse<string>.Ok(await _doctorRepository.GenerateDoctorIdAsync());
    }

    public async Task<ApiResponse<DoctorDto>> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
    {
        if (string.IsNullOrWhiteSpace(createDoctorDto.DoctorId))
        {
            createDoctorDto.DoctorId = await _doctorRepository.GenerateDoctorIdAsync();
        }

        var entity = new DoctorEntity
        {
            DoctorId = createDoctorDto.DoctorId,
            UserId = createDoctorDto.UserId,
            FirstName = createDoctorDto.FirstName,
            LastName = createDoctorDto.LastName,
            Specialization = createDoctorDto.Specialization,
            Email = createDoctorDto.Email,
            Phone = createDoctorDto.Phone,
            YearsOfExperience = createDoctorDto.YearsOfExperience,
            IsActive = true
        };

        var created = await _doctorRepository.CreateDoctorAsync(entity);
        return ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(created), "Doctor created successfully");
    }

    public async Task<ApiResponse<DoctorDto>> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto)
    {
        var entity = new DoctorEntity
        {
            DoctorId = updateDoctorDto.DoctorId,
            UserId = updateDoctorDto.UserId,
            FirstName = updateDoctorDto.FirstName,
            LastName = updateDoctorDto.LastName,
            Specialization = updateDoctorDto.Specialization,
            Email = updateDoctorDto.Email,
            Phone = updateDoctorDto.Phone,
            YearsOfExperience = updateDoctorDto.YearsOfExperience,
            IsActive = updateDoctorDto.IsActive
        };

        var updated = await _doctorRepository.UpdateDoctorAsync(id, entity);
        return updated is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(updated), "Doctor updated successfully");
    }

    public async Task<ApiResponse<string>> DeleteDoctorAsync(int id)
    {
        var deleted = await _doctorRepository.DeleteDoctorAsync(id);
        return deleted ? ApiResponse<string>.Ok("Doctor deleted successfully") : ApiResponse<string>.Fail("Doctor not found");
    }
}
